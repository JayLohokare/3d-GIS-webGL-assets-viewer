using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using OrchestrationService.Data;

namespace OrchestrationService
{
    public class Orchestration
    {
        private readonly OrchestrationDataContext _dbContext;
        public Orchestration(OrchestrationDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("Orchestration")]
        public void Run([ServiceBusTrigger("AUTOUD_ORCHESTRATION", "AUTOUD_ORCHESTRATION", Connection = "ServiceBusConnectionString")]string serviceBusMessage, ILogger log)
        {
            //parse incoming message
            var eventMessage = new EventMessage(serviceBusMessage);
            log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - Orchestration service received message from {eventMessage.CreatedBySystem}");
            //get event service maps based on event type
            var eventServiceMapList = _dbContext.EventServiceMapView.Where(p => p.EventId.ToLower() == eventMessage.EventType.ToLower()).OrderBy(p => p.ExecutionOrder);
            var eventServiceMapActionList = new List<EventServiceMapView>();
            bool originatorFound = false;
            
            foreach(var eventServiceMapRecord in eventServiceMapList)
            {
               if(!originatorFound)
               {
                    if(eventServiceMapRecord.ServiceId.ToLower() == eventMessage.CreatedBySystem.ToLower())
                    {
                        originatorFound = true;
                    }
               } 
               else
               {
                    eventServiceMapActionList.Add(eventServiceMapRecord);
                    if (eventServiceMapRecord.NotifyCompletion)
                        break;
               }
            }
            
            if(eventServiceMapActionList.Count > 0)
            {
                foreach (var eventServiceMapRecord in eventServiceMapActionList)
                {
                    var topicClient = new TopicClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"), eventServiceMapRecord.ServiceBusTopicId);
                    var toSendEventMessage = new EventMessage(serviceBusMessage);
                    toSendEventMessage.NotifyCompletion = eventServiceMapRecord.NotifyCompletion;
                    var toSendEventMessageJson = JsonConvert.SerializeObject(toSendEventMessage);
                    var message = new Message
                    {
                        ContentType = "application/json",
                        Body = Encoding.UTF8.GetBytes(toSendEventMessageJson),
                        CorrelationId = eventMessage.CorrelationId.Value.ToString(),
                        Label = string.Format("AUTOUD From ORCHESTRATION To {0}", eventServiceMapRecord.ServiceId.ToUpper())
                    };
                    topicClient.SendAsync(message);
                    log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - Orchestration service sent message to {eventServiceMapRecord.ServiceBusTopicId} topic");
                }
            }
            
            log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - Orchestration service finishing processing message");
        }
    }
}
