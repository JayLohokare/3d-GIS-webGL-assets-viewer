using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Linq;
using RestSharp.Serialization.Json;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace ModelConversionService
{
    public static class GetToken
    {
        [FunctionName("GetToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get",  Route = "testConnectivity")] HttpRequest reqo,
            ILogger log)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                string url = "https://developer.api.autodesk.com/authentication/v1/authenticate";

                var dict = new Dictionary<string, string>();

                dict.Add("client_id", Environment.GetEnvironmentVariable("AUTODESK_CLIENT_ID"));
                dict.Add("client_secret", Environment.GetEnvironmentVariable("AUTODESK_CLIENT_SECRET"));
                dict.Add("grant_type", "client_credentials");
                dict.Add("scope", "code:all data:write data:read bucket:create bucket:delete bucket:read");

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(dict) };
                HttpResponseMessage res = await httpClient.SendAsync(req);
                res.EnsureSuccessStatusCode();

                string responseBody = await res.Content.ReadAsStringAsync();

                var jsonResponseContent = (JObject)JsonConvert.DeserializeObject(responseBody);
                string responseToken = jsonResponseContent["access_token"].Value<string>();

                httpClient.CancelPendingRequests();
                return new OkObjectResult(responseToken);
            }
            catch (Exception ex)
            {
                httpClient.CancelPendingRequests();
                Console.WriteLine(ex.ToString());
                return new OkObjectResult(ex.ToString());
            }

        }
    }
}
