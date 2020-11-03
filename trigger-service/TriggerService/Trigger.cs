using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using System.Text;
using TriggerService.Service;

namespace TriggerService
{
    public static class Trigger
    {

        [FunctionName("Trigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "trigger")] HttpRequest req,
            ILogger log)
        {
            string triggermessage = await new StreamReader(req.Body).ReadToEndAsync();
            var response = ServiceBusService.SendMessageToOrchestrationService(triggermessage, null);
            if(response.Success)
                log.LogInformation($"CorrelationId: {response.CorrelationId} - Trigger service sent message to orchestration");
            else
                log.LogError($"CorrelationId: {response.CorrelationId} - Trigger service failed to send message to orchestration");
            return new OkObjectResult(response);
        }
    }
}
