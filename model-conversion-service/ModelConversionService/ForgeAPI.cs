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

    public class ForgeAPI
    {
        public static async Task<string> GetForgeToken()
        {
            HttpClient httpClient = new HttpClient();
            string url = "https://developer.api.autodesk.com/authentication/v1/authenticate";

            var dict = new Dictionary<string, string>();
                
            dict.Add("client_id", Environment.GetEnvironmentVariable("AUTODESK_CLIENT_ID"));
            dict.Add("client_secret",  Environment.GetEnvironmentVariable("AUTODESK_CLIENT_SECRET"));
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
            return responseToken;
        }


        public static async Task<bool> CreateOSSBucket(string accessToken, string bucketName)
        {
            HttpClient httpClient = new HttpClient();
            string url = "https://developer.api.autodesk.com/oss/v2/buckets";

            string authString = "Bearer " + accessToken;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authString);

            var dict = new Dictionary<string, string>();
            dict.Add("bucketKey", bucketName);
            dict.Add("policyKey", "transient");

            var json = JsonConvert.SerializeObject(dict);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage res = await httpClient.PostAsync(url, data);

            res.EnsureSuccessStatusCode();

            string responseBody = await res.Content.ReadAsStringAsync();
           
            var jsonResponseContent = (JObject)JsonConvert.DeserializeObject(responseBody);
            string bucketKey = jsonResponseContent["bucketKey"].Value<string>();

            httpClient.CancelPendingRequests();
            return true;
           
        }


        public static async Task<bool> CheckOSSBucket(string accessToken, string bucketName)
        {
            
            HttpClient httpClient = new HttpClient();

            string url = "https://developer.api.autodesk.com/oss/v2/buckets/" + bucketName + "/details";

            string authString = "Bearer " + accessToken;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authString);

            var req = new HttpRequestMessage(HttpMethod.Get, url) { };

            HttpResponseMessage res = await httpClient.SendAsync(req);
            if (res.IsSuccessStatusCode){
                string responseBody = await res.Content.ReadAsStringAsync();
                
                var jsonResponseContent = (JObject)JsonConvert.DeserializeObject(responseBody);
                string bucketKey = jsonResponseContent["bucketKey"].Value<string>();

                httpClient.CancelPendingRequests();
                Console.WriteLine("Bucket already exists");
                return true;
            }
            else
            {
                return false;
            }
            
        }



        public static async Task<string> UploadFileToOSS(string accessToken, string bucketName, string FileName, Stream FileData)
        {
            HttpClient httpClient = new HttpClient();
            
            string url = "https://developer.api.autodesk.com/oss/v2/buckets/" + bucketName + "/objects/" + FileName;

            string authString = "Bearer " + accessToken;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authString);

            var data = new StreamContent(FileData);

            HttpResponseMessage res = await httpClient.PutAsync(url, data);
            res.EnsureSuccessStatusCode();

            string responseBody = await res.Content.ReadAsStringAsync();
            
            var jsonResponseContent = (JObject)JsonConvert.DeserializeObject(responseBody);
            string fileObjectID = jsonResponseContent["objectId"].Value<string>();

            httpClient.CancelPendingRequests();
            return fileObjectID;
            
        }


        public static async Task<bool> InitiateTranslation(string accessToken, string bucketName, string FileID, string outputType)
        {
            
            HttpClient httpClient = new HttpClient();

           
            var plainTextBytes = Encoding.UTF8.GetBytes(FileID);
            var base64URN = Convert.ToBase64String(plainTextBytes);

            string url = "https://developer.api.autodesk.com/modelderivative/v2/designdata/job";

            string authString = "Bearer " + accessToken;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authString);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");


            //TODO Change this from hardcoded string to proper structure 
            var jsonBodyString = @"{
                    'input':{
                        'urn': '" + base64URN +
                    @"'},
                    'output': {
                        'destination': {'region': 'us'},
                        'formats': [{'type': '" + outputType + @"'} ]
                    }
                }";


            JObject jsonObj = JObject.Parse(jsonBodyString);
            var data = new StringContent(jsonObj.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage res = await httpClient.PostAsync(url, data);
            res.EnsureSuccessStatusCode();

            string responseBody = await res.Content.ReadAsStringAsync();
            
            var jsonResponseContent = (JObject)JsonConvert.DeserializeObject(responseBody);
            string status = jsonResponseContent["result"].Value<string>();


            if ((status == "success") || (status == "created"))
            {
                httpClient.CancelPendingRequests();
                return true;
            }

            httpClient.CancelPendingRequests();
            return false;
           
        }



        public static async Task<int> CheckTranslationStatus(string accessToken, string bucketName, string FileID)
        {
            HttpClient httpClient = new HttpClient();

            var plainTextBytes = Encoding.UTF8.GetBytes(FileID);
            var base64URN = Convert.ToBase64String(plainTextBytes);

            string url = "https://developer.api.autodesk.com/modelderivative/v2/designdata/" + base64URN + "/manifest";

            string authString = "Bearer " + accessToken;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authString);

            var req = new HttpRequestMessage(HttpMethod.Get, url) { };

            HttpResponseMessage res = await httpClient.SendAsync(req);
            res.EnsureSuccessStatusCode();

            string responseBody = await res.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody);
            
            var jsonResponseContent = (JObject)JsonConvert.DeserializeObject(responseBody);

            var progressStatus = jsonResponseContent["progress"].Value<string>();
            var successStatus = jsonResponseContent["status"].Value<string>();
            //var progressStatus = derivatives["progress"].Value<string>();

            if (progressStatus == "complete")
            {
                if (successStatus == "success")
                {
                    httpClient.CancelPendingRequests();
                    return 1;
                }
                else
                {
                    httpClient.CancelPendingRequests();
                    return 2;
                }
            }
            else
            {
                httpClient.CancelPendingRequests();
                return 0;
            }
            
        }

        public static async Task<int> SaveConversionOutputsToBlob(string accessToken, string bucketName, string FileID)
        {
            HttpClient httpClient = new HttpClient();

            var plainTextBytes = Encoding.UTF8.GetBytes(FileID);
            var base64URN = Convert.ToBase64String(plainTextBytes);

            string url = "https://developer.api.autodesk.com/modelderivative/v2/designdata/" + base64URN + "/manifest";

            string authString = "Bearer " + accessToken;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authString);

            var req = new HttpRequestMessage(HttpMethod.Get, url) { };

            HttpResponseMessage res = await httpClient.SendAsync(req);
            res.EnsureSuccessStatusCode();

            string responseBody = await res.Content.ReadAsStringAsync();
            
            JObject jsonResponseContent = (JObject)JsonConvert.DeserializeObject(responseBody);

            string FileURN = (string)jsonResponseContent["urn"];

            JArray derivatiesObject = (JArray)jsonResponseContent["derivatives"];

            //< Dictionary<string, List<Dictionary<string, List<Dictionary<string, string>>>>> >
            var derivatiesObjectContent = derivatiesObject[0].ToString();
            
            var responseObject = (JObject)JsonConvert.DeserializeObject(derivatiesObjectContent);
            if (responseObject == null)
            {
                httpClient.CancelPendingRequests();
                return 0;
            }

            JArray outputFiles = (JArray)responseObject["children"];
            

            for (int i = 0; i < outputFiles.Count; i++)
            {
                var outputFile = outputFiles[i];
                string outputURN = outputFile["urn"].ToString();
                string convertedModelFileName = outputFile["urn"].ToString().Split('/').Last();
                if(await ForgeAPI.SaveConvertedModelToBlob(accessToken, bucketName, FileURN, outputURN, convertedModelFileName) != 1)
                {
                    return 0;
                }
            }

            httpClient.CancelPendingRequests();
            return 1;

        }


        public static async Task<int> SaveConvertedModelToBlob(string accessToken, string bucketName, string fileURN, string modelURN, string modelFileName)
        {
            HttpClient httpClient = new HttpClient();

            var url = "https://developer.api.autodesk.com/modelderivative/v2/designdata/" + fileURN + "/manifest/" + modelURN;
           
            string authString = "Bearer " + accessToken;
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authString);

            var req = new HttpRequestMessage(HttpMethod.Get, url) { };
            HttpResponseMessage res = await httpClient.SendAsync(req);
            res.EnsureSuccessStatusCode();

            var myBlob = await res.Content.ReadAsStreamAsync();

            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            // Check whether the connection string can be parsed.
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                CloudBlobClient blobClient;
                CloudBlobContainer container;

                blobClient = storageAccount.CreateCloudBlobClient();

                container = blobClient.GetContainerReference(Environment.GetEnvironmentVariable("BlobStorageConvertedModelsContainer"));

                await container.CreateIfNotExistsAsync();

                CloudBlockBlob blob;
                string name;

                name = modelFileName;
                blob = container.GetBlockBlobReference(name);

                using (Stream stream = myBlob)
                {
                    await blob.UploadFromStreamAsync(stream);
                }
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add an environment variable named 'AZURE_STORAGE_CONNECTION_STRING' with your storage " +
                    "connection string as a value.");
                httpClient.CancelPendingRequests();
                return 0;
            }

            httpClient.CancelPendingRequests();
            return 1;

           
        }


    }
}