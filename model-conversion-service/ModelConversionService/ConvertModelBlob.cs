using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ModelConversionService
{
    public static class ConvertModelBlob
    {

        [FunctionName("ConvertModelBlob")]
        public static async System.Threading.Tasks.Task RunAsync([BlobTrigger("pplz-cont-autoud3d-d/{name}", Connection = "BlobStorageConnectionString")] Stream myBlob, string name, ILogger log)
        {
            //TODO Create graceful handler for different types of files
            //Only handle IPT files for now
            if (name.Split('.').Last() != "ipt")
            {
                return;
            }

            string ossBucketName = Environment.GetEnvironmentVariable("AUTODESK_OSS_BUCKET");

            try
            {
                //Get Forge token
                string token = await ForgeAPI.GetForgeToken();
                if (token == null)
                {
                    return;
                }

                //Check if forge bucket exists; If not then create it
                if (!await ForgeAPI.CheckOSSBucket(token, ossBucketName))
                {
                    if (!await ForgeAPI.CreateOSSBucket(token, ossBucketName))
                    {
                        return;
                    }
                }

                //Upload file
                string fileID = await ForgeAPI.UploadFileToOSS(token, ossBucketName, name, myBlob);
                if (fileID == null)
                {
                    return;
                }

                //Initiate translation
                if (!await ForgeAPI.InitiateTranslation(token, ossBucketName, fileID, "obj"))
                {
                    return;
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
                    return;
                }

                var success = ForgeAPI.SaveConversionOutputsToBlob(token, ossBucketName, fileID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
