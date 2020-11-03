using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Thinktecture.IdentityModel.Client;

namespace EAMService.Service
{
    class InforEAMService
    {
        #region Environment properties

        private static string OrganizationCode = "";
        private static string TenantId = "";

        private static string IONAPIBaseUrl = "<replace with ia+'/'+ti from .ionapi file>";
        private static string ResourceOwnerClientId = "<replace with ci from .ionapi file>";
        private static string ResourceOwnerClientSecret = "<replace with cs from .ionapi file>";

        private static string OAuth2TokenEndpoint = "<replace with pu+ot from .ionapi file>";
        private static string OAuth2TokenRevocationEndpoint = "<replace with pu+or from .ionapi file>";
        private static string OAuth2AuthorizationEndpoint = "<replace with pu+oa from .ionapi file>";

        #endregion

        #region User Properties

        private static string ServiceAccountAccessKey = "<replace with saak from .ionapi file>";
        private static string ServiceAccountSecretKey = "<replace with sask from .ionapi file>";

        #endregion

        private static OAuth2Client _oauth2;
        private static string _token;
        private static DateTime _tokenExpiry;
        public InforEAMService()
        {
            OrganizationCode = Environment.GetEnvironmentVariable("OrganizationCode");
            TenantId = Environment.GetEnvironmentVariable("TenantId");

            IONAPIBaseUrl = Environment.GetEnvironmentVariable("IONAPIBaseUrl");
            ResourceOwnerClientId = Environment.GetEnvironmentVariable("ResourceOwnerClientId");
            ResourceOwnerClientSecret = Environment.GetEnvironmentVariable("ResourceOwnerClientSecret");
            OAuth2TokenEndpoint = Environment.GetEnvironmentVariable("OAuth2TokenEndpoint");
            OAuth2TokenRevocationEndpoint = Environment.GetEnvironmentVariable("OAuth2TokenRevocationEndpoint");
            OAuth2AuthorizationEndpoint = Environment.GetEnvironmentVariable("OAuth2AuthorizationEndpoint");
            ServiceAccountAccessKey = Environment.GetEnvironmentVariable("ServiceAccountAccessKey");
            ServiceAccountSecretKey = Environment.GetEnvironmentVariable("ServiceAccountSecretKey");
            Init();
        }
        public void Init()
        {
            _oauth2 = new OAuth2Client(new Uri(OAuth2TokenEndpoint), ResourceOwnerClientId, ResourceOwnerClientSecret);

            //Request a token with the provided ServiceAccountAccessKey and ServiceAccountSecretKey
            TokenResponse token = RequestToken();
            UpdateTokenValues(token);
        }

        private void UpdateTokenValues(TokenResponse token)
        {
            _token = token.AccessToken;
            //set expiry 2 mins before
            _tokenExpiry = DateTime.Now.AddSeconds(token.ExpiresIn - 120);
        }

        public void CallService()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(IONAPIBaseUrl)
            };
            if (DateTime.Now > _tokenExpiry)
                UpdateTokenValues(RequestToken());
            client.SetBearerToken(_token);

            //this GET call works fine with 200 response code
            //var response = client.GetAsync("https://mingle35-ionapi.inforgov.com/PPLEUC_DEM/EAM/APIServices").Result;
            //Console.WriteLine(response);
            //Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            // Create SOAP request for the API call
            HttpContent httpContent = new StringContent(ConstructSoapRequest("Get.AssetEquipment"));
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, "/PPLEUC_DEM/EAM/APIServices?tenant=PPLEUC_DEM")
            {
                Content = httpContent
            };
            httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml; charset=utf-8");
            httpRequest.Content.Headers.Add("X-TenantId", TenantId);

            // Return value
            var response = client.SendAsync(httpRequest).Result;
            Console.WriteLine(response);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);

        }

        private static string ConstructSoapRequest(string bodType)
        {
            string messageID = Guid.NewGuid().ToString();
            string soapenvStart = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:mp=""http://schemas.datastream.net/MP_functions"" xmlns:sec=""http://schemas.xmlsoap.org/ws/2002/04/secext"" xmlns:mp0=""http://schemas.datastream.net/MP_functions/MP0302_001"" xmlns:mp1=""http://schemas.datastream.net/MP_fields"">";
            string soapenvHeaderTemplate = @"<soapenv:Header>
                                                <mp:Tenant>{0}</mp:Tenant>
                                                <mp:Organization>{1}</mp:Organization>
                                                <mp:BODType>{2}</mp:BODType>
                                                <mp:MessageID>{3}</mp:MessageID>
                                                <sec:Security>
                                                    <sec:UsernameToken>
                                                        <sec:Username>{4}</sec:Username>
                                                        <sec:Password>{5}</sec:Password>
                                                    </sec:UsernameToken>
                                                    <sec:SecurityTokenReference>
                                                        <sec:Embedded/>
                                                    </sec:SecurityTokenReference>
                                                </sec:Security>
                                                <mp:FromLogicalID>lid://infor.ws.inforgov.com?tenant={0}</mp:FromLogicalID>
                                                <mp:ToLogicalID>lid://infor.eam.inforgov.com?tenant={0}</mp:ToLogicalID>
                                            </soapenv:Header>";
            //Possible other xml elements based on documentation
            //<mp:MessageConfig></mp:MessageConfig>
            //<mp:SessionScenario></mp:SessionScenario>
            //<mp:Session></mp:Session>
            //<mp:FromLogicalID>lid://default</mp:FromLogicalID>
            //<mp:ToLogicalID>lid://default</mp:ToLogicalID>

            string soapenvHeader = string.Format(soapenvHeaderTemplate, TenantId, OrganizationCode, bodType, messageID, ServiceAccountAccessKey, ServiceAccountSecretKey);
            string soapenvBody = @"<soapenv:Body>
                                        <mp0:MP0302_GetAssetEquipment_001 verb=""Get"" noun=""AssetEquipment"" version=""001"">
                                            <mp1:ASSETID>
                                                <mp1:EQUIPMENTCODE>0102EBE4-80B0-47F0-9D95-840AB9</mp1:EQUIPMENTCODE>
                                            </mp1:ASSETID>
                                        </mp0:MP0302_GetAssetEquipment_001>
                                    </soapenv:Body>";
            string soapenvEnd = "</soapenv:Envelope>";
            string xmlSOAP = soapenvStart + soapenvHeader + soapenvBody + soapenvEnd;
            return xmlSOAP;
        }

        private static TokenResponse RequestToken()
        {
            return _oauth2.RequestResourceOwnerPasswordAsync
                (ServiceAccountAccessKey, ServiceAccountSecretKey).Result;
        }
    }
}
