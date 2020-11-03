using System;
using System.Text;
using Common.Data;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UDHService.Data;

namespace UDHService
{
    public class UDHServiceBus
    {
        public static readonly string SERVICE_ID = "UDH";

        private readonly UDHDataContext _dbContext;
        public UDHServiceBus(UDHDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("UDHServiceBus")]
        public void Run([ServiceBusTrigger("AUTOUD_UDH", "AUTOUD_UDH", Connection = "ServiceBusConnectionString")]string serviceBusMessage, ILogger log)
        {
            //get the message
            var eventMessage = new EventMessage(serviceBusMessage);
            log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} service recieved message");

            //handle the message
            var workRequest = JsonConvert.DeserializeObject<WorkRequest>(eventMessage.Message);
            _dbContext.AddOrUpdateWorkorder(workRequest);

            //Send back response to Orchastration if needed
            if (eventMessage.NotifyCompletion)
            {
                var topicClient = new Microsoft.Azure.ServiceBus.TopicClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"), Environment.GetEnvironmentVariable("OrchestrationServiceTopicId"));
                eventMessage.CreatedBySystem = SERVICE_ID;
                var toSendEventMessageJson = JsonConvert.SerializeObject(eventMessage);
                var message = new Message
                {
                    ContentType = "application/json",
                    Body = Encoding.UTF8.GetBytes(toSendEventMessageJson),
                    CorrelationId = eventMessage.CorrelationId.Value.ToString(),
                    Label = string.Format("AUTOUD From {0} To ORCHESTRATION", SERVICE_ID)
                };
                topicClient.SendAsync(message);
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} service sent message to {Environment.GetEnvironmentVariable("OrchestrationServiceTopicId")} topic");
            }
        }
    }
}
