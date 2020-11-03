using Common.Data;
using GISService.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace GISService.Services
{
    class ArcGISService
    {
        private readonly string ServerUrl = Environment.GetEnvironmentVariable("ArcGISServerUrl");
        private readonly string ClientId = Environment.GetEnvironmentVariable("ArcGISClientId");
        private readonly string ClientSecret = Environment.GetEnvironmentVariable("ArcGISClientSecret");
        private readonly string ServerUrl_TempStage = Environment.GetEnvironmentVariable("ArcGISServerUrl_TempStage");
        private readonly string ClientId_TempStage = Environment.GetEnvironmentVariable("ArcGISClientId_TempStage");
        private readonly string ClientSecret_TempStage = Environment.GetEnvironmentVariable("ArcGISClientSecret_TempStage");
        private readonly string IgnoreSSLCert = Environment.GetEnvironmentVariable("IgnoreSSLCert");
        private readonly string LIFECYCLESTATUS_FIELDNAME = "LIFECYCLESTATUS";
        private readonly string PREFERRED_SPATIAL_REFERENCE = "4326";
        //TODO: support pagination
        public static readonly string MAX_FEATURES_COUNT = "300";

        private static TokenInfo _tokenInfo;
        private static TokenInfo _tokenInfo_TempStage;
        public ArcGISService()
        {
            if(IgnoreSSLCert == "1")
            {
                //TODO: Remove after fixing SSL issue with self-signed cert on ArcGIS Dev server
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }
        }

        private TokenInfo Authenicate(FeatureServerRequest featureServerRequest)
        {
            var tokenInfo = featureServerRequest.IsStaged ? _tokenInfo_TempStage : _tokenInfo;
            var serverUrl = featureServerRequest.IsStaged ? ServerUrl_TempStage : ServerUrl;
            var clientId = featureServerRequest.IsStaged ? ClientId_TempStage : ClientId;
            var clientSecret = featureServerRequest.IsStaged ? ClientSecret_TempStage : ClientSecret;

            if (tokenInfo != null && tokenInfo.ExpiresDate!= null && tokenInfo.ExpiresDate > DateTime.Now)
                return tokenInfo;

            try
            {
                var client = new RestClient($"{GetServerUrl(featureServerRequest)}portal/sharing/rest/generateToken?f=json");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AlwaysMultipartFormData = true;
                request.AddParameter("username", clientId);
                request.AddParameter("password", clientSecret);
                request.AddParameter("referer", serverUrl);
                IRestResponse response = client.Execute(request);
                tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(response.Content.ToString());
                tokenInfo.ExpiresDate = DateTimeUtils.UnixTimeStampToDateTime(double.Parse(tokenInfo.Expires)).AddSeconds(-60);
                if (featureServerRequest.IsStaged)
                    _tokenInfo_TempStage = tokenInfo;
                else
                    _tokenInfo = tokenInfo;
                return tokenInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        private void SetupAuthentication(RestClient client, FeatureServerRequest featureServerRequest)
        {
            var tokenInfo = Authenicate(featureServerRequest);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", tokenInfo.Token));

        }

        private string GetServerUrl(FeatureServerRequest request)
        {
            return request.IsStaged? ServerUrl_TempStage : ServerUrl;
        }

        internal string GetFeatureServerList(FeatureServerRequest featureServerRequest)
        {
            var client = new RestClient($"{GetServerUrl(featureServerRequest)}server/rest/services/{featureServerRequest.FeatureServerPath}/FeatureServer?f=json");
            SetupAuthentication(client, featureServerRequest);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        internal string GetFeatures(FeaturesRequest featuresRequest)
        {
            var whereClause = "where=1=1&";
            if (featuresRequest.AssetIds != null && featuresRequest.AssetIds.Count > 0)
            {
                whereClause = "where=GLOBALID IN(";
                foreach (var assetId in featuresRequest.AssetIds)
                {
                    whereClause += "'{" + assetId.ToString().ToUpper() + "}',";
                }
                // remove last ,
                whereClause = whereClause.Substring(0, whereClause.Length -1);
                whereClause += ")&";
            }
            var geometryClause = "";
            if (featuresRequest.Viewport != null && featuresRequest.Viewport.Count == 4)
            {
                geometryClause = "geometry={\"rings\":[[[" + featuresRequest.Viewport[0][0] + "," + featuresRequest.Viewport[0][1] + "]," + 
                                                    "[" + featuresRequest.Viewport[1][0] + "," + featuresRequest.Viewport[1][1] + "]," +
                                                    "[" + featuresRequest.Viewport[2][0] + "," + featuresRequest.Viewport[2][1] + "]," +
                                                    "[" + featuresRequest.Viewport[3][0] + "," + featuresRequest.Viewport[3][1] + "]" +
                                                    "]],spatialReference:{wkid:" + PREFERRED_SPATIAL_REFERENCE + "}}&";
            }
            var countParam = "";
            if (featuresRequest.MaxCount.HasValue)
            {
                if(featuresRequest.MaxCount.Value != -1)
                    countParam = $"&resultRecordCount={featuresRequest.MaxCount.Value}";
            }
            else
                countParam = $"&resultRecordCount={MAX_FEATURES_COUNT}";

            var url = $"{GetServerUrl(featuresRequest)}server/rest/services/{featuresRequest.FeatureServerPath}/FeatureServer/{featuresRequest.LayerId}/query?{whereClause}{geometryClause}geometryType=esriGeometryPolygon&spatialRel=esriSpatialRelIntersects&outFields=*&returnGeometry=true&outSR={PREFERRED_SPATIAL_REFERENCE}&returnDistinctValues=false&returnZ=true&featureEncoding=esriDefault&f=pjson{countParam}";
            //need to html encode [ & ]
            url = url.Replace("[", "%5B");
            url = url.Replace("]", "%5D");
            var client = new RestClient(url);
            SetupAuthentication(client, featuresRequest);

            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        internal string GetAssetData(AssetRequest assetRequest)
        {
            var client = new RestClient($"{GetServerUrl(assetRequest)}server/rest/services/{assetRequest.FeatureServerPath}/FeatureServer/{assetRequest.LayerId}/query?f=json&returnGeometry=true&spatialRel=esriSpatialRelIntersects&objectIds={assetRequest.GISObjectId}&outFields=*&outSR={PREFERRED_SPATIAL_REFERENCE}");
            SetupAuthentication(client, assetRequest);
            var request = new RestRequest(Method.GET);
           
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        internal string GetAssetTrace(TraceRequest traceRequest)
        {
            var traceConfiguration = "traceConfiguration={\"subnetworkName\":\"" + traceRequest.SubnetworkName + "\"}";
            var client = new RestClient($"{GetServerUrl(traceRequest)}server/rest/services/Electric_Utility_Network/UtilityNetworkServer/trace?traceType=subnetwork&traceLocations=[]&{traceConfiguration}&resultType=&f=pjson");
            SetupAuthentication(client, traceRequest);
            var request = new RestRequest(Method.GET);

            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        internal string GetFeatureServerSubnetworks(FeatureServerRequest featureServerRequest)
        {
            var client = new RestClient($"{GetServerUrl(featureServerRequest)}server/rest/services/{featureServerRequest.FeatureServerPath}/FeatureServer/{featureServerRequest.LayerId}/query?f=json&where=1=1&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields={featureServerRequest.FeatureServerFields}&outSR={PREFERRED_SPATIAL_REFERENCE}");
            SetupAuthentication(client, featureServerRequest);
            var request = new RestRequest(Method.GET);
            
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        internal Dictionary<string,int> GetFeatureServerStatusDictionary(FeatureServerRequest featureServerRequest)
        {
            var statusCodes = new Dictionary<string, int>();
            var client = new RestClient($"{GetServerUrl(featureServerRequest)}server/rest/services/{featureServerRequest.FeatureServerPath}/FeatureServer/{featureServerRequest.LayerId}?f=pjson");
            SetupAuthentication(client, featureServerRequest);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var jToken = JsonConvert.DeserializeObject<JToken>(response.Content);
            foreach(var token in jToken["fields"])
            {
                if(token["name"].ToString().ToUpper() == LIFECYCLESTATUS_FIELDNAME)
                {
                    foreach(var codeValue in token["domain"]["codedValues"])
                    {
                        statusCodes.Add(codeValue["name"].ToString(), int.Parse(codeValue["code"].ToString()));
                    }
                }
            }
            return statusCodes;
        }

        internal string UpdateAsset(AssetRequest request)
        {
            // Logic to update asset including status
            throw new NotImplementedException();
        }
    }
}
