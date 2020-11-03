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

namespace ModelConversionService
{


    public static class ConvertModelHTTP
    {
        [FunctionName("ConvertModelHTTP")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "convert")] HttpRequest req,
            ILogger log)
        {


            var rawFileStream = await new StreamReader(req.Body).ReadToEndAsync();
            
            byte[] byteArray = Encoding.ASCII.GetBytes(rawFileStream);
            MemoryStream uploadedFileMS = new MemoryStream(byteArray);
            Stream uploadedFile = uploadedFileMS;

            string name = req.Query["name"];
            string conversionType = "obj";

            if (!string.IsNullOrEmpty(req.Query["type"].ToString()))
            {
                conversionType = req.Query["type"];
            }

            Console.WriteLine("Conversion type");
            Console.WriteLine(conversionType);
           

            string successMessage = "Success";
            string failureMessage = "ERROR: Model conversion service failed";

            string ossBucketName = Environment.GetEnvironmentVariable("AUTODESK_OSS_BUCKET");

            try
            {
                //Get Forge token
                string token = await ForgeAPI.GetForgeToken();
                if (token == null)
                {
                    return new OkObjectResult(failureMessage);
                }

                //Check if forge bucket exists; If not then create it
                if (await ForgeAPI.CheckOSSBucket(token, ossBucketName) == false)
                {
                    if (await ForgeAPI.CreateOSSBucket(token, ossBucketName) != true)
                    {
                        return new OkObjectResult(failureMessage);
                    }
                }

                //Upload file
                string fileID = await ForgeAPI.UploadFileToOSS(token, ossBucketName, name, uploadedFile);
                if (fileID == null)
                {
                    return new OkObjectResult(failureMessage);
                }

                //Initiate translation
                if (!await ForgeAPI.InitiateTranslation(token, ossBucketName, fileID, conversionType))  
                {
                    return new OkObjectResult(failureMessage);
                }

                //Keep checking for conversion success
                var status = await ForgeAPI.CheckTranslationStatus(token, ossBucketName, fileID);
                while (status == 0)
                {
                    Thread.Sleep(1000 * 5);
                    status = await ForgeAPI.CheckTranslationStatus(token, ossBucketName, fileID);
                }
                //Terminate  if conversion failed
                if (status == 2)
                {
                    return new OkObjectResult(failureMessage);
                }

                var success = ForgeAPI.SaveConversionOutputsToBlob(token, ossBucketName, fileID);

                return new OkObjectResult(successMessage);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return new OkObjectResult(ex.ToString());
            }
        }

    }
}
