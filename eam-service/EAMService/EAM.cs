using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using EAMService.Service;
using Common.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using System;
using System.Text;

namespace EAMService
{
    public static class EAM
    {
        public static readonly string SERVICE_ID = "EAM";
        public static readonly List<string> GetAssetEvents = new List<string>() { "3D_GET_ASSET_EAM" };

        [FunctionName("EAM")]
        public static void Run([ServiceBusTrigger("AUTOUD_EAM", "AUTOUD_EAM", Connection = "ServiceBusConnectionString")]string serviceBusMessage, ILogger log)
        {
            var eventMessage = new EventMessage(serviceBusMessage);

            //GET Asset data
            if (GetAssetEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetRequest>(eventMessage.Message);
                AssetEamResponse response = new AssetEamResponse() { AssetId = request.AssetId };
                
                // hardcoded value for now, should use InforEAMService here
                if (request.AssetId.ToString().ToLower() == "7793b82d-88c1-4d7b-9b51-867259a00753")
                {
                    response.Make = "Make 1";
                    response.Model = "Model 1";
                    response.SerialNumber = "1234";
                    response.AssetType = "Substation";
                    response.Latitude = 40.626022;
                    response.Longitude = -75.549500;
                }
                else if (request.AssetId.ToString().ToLower() == "9d22824d-bcf5-4195-8aeb-a51733133f75")
                {
                    response.Make = "Make 2";
                    response.Model = "Model 2";
                    response.SerialNumber = "5678";
                    response.AssetType = "Substation";
                    response.Latitude = 40.630926;
                    response.Longitude = -75.561110;
                }
                else
                {
                    response.Make = "Make 3";
                    response.Model = "Model 3";
                    response.SerialNumber = "8910";
                    response.AssetType = "Substation";
                    response.Latitude = 40.638774;
                    response.Longitude = -75.539037;
                }

                eventMessage.Message = JsonConvert.SerializeObject(response);
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"C# ServiceBus topic trigger function processed message: {serviceBusMessage}");
            }

            //Send back response to Orchastration if needed
            if (eventMessage.NotifyCompletion)
            {
                var topicClient = new TopicClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"), Environment.GetEnvironmentVariable("OrchestrationServiceTopicId"));
                var toSendEventMessageJson = JsonConvert.SerializeObject(eventMessage); 
                var message = new Message
                {
                    ContentType = "application/json",
                    Body = Encoding.UTF8.GetBytes(toSendEventMessageJson),
                    CorrelationId = eventMessage.CorrelationId.Value.ToString(),
                    Label = string.Format("AUTOUD From {0} To ORCHESTRATION", SERVICE_ID)
                };
                topicClient.SendAsync(message);
            }
        }
    }
}
