using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using ModelService.Services;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace ModelService
{
    public class ModelsFileService
    {
        private readonly IStorageSevice _storageSevice;
        public ModelsFileService(IStorageSevice storageSevice)
        {
            _storageSevice = storageSevice;
        }

        private static string MODEL_NOT_FOUND_MESSAGE = "Model ID not valid. Please pass a valid Model ID";

        [FunctionName("ModelFiles")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "modelfiles/")] HttpRequest req, 
            ILogger log)
        {
            var fileName = req.Query["model_name"];

            if (fileName != "")
            {
                var content = _storageSevice.GetBlobFile(fileName);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(content);
                response.Content.Headers.Add("Content-Disposition", "attachment;  filename*=UTF-8''" + Uri.EscapeDataString(fileName));
                return response;
            }
            else
            {
                log.LogWarning(string.Format("{0}: {1}. Model NAME={2}", req.Method, MODEL_NOT_FOUND_MESSAGE, fileName));
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
