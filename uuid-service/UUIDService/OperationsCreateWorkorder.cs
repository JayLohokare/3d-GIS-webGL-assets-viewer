using System;
using System.Linq;
using System.Text;
using Common.Data.DTO;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UUIDService
{
    public class OperationsCreateWorkorder
    {
        public static readonly string SERVICE_ID = "REGISTRY";
        public static readonly string EVENT_TYPE_ID = "REGISTRY_CREATE_WORKORDER_EAM";
        private readonly UUIDDataContext _DbContext;
        public OperationsCreateWorkorder(UUIDDataContext dbContext)
        {
            _DbContext = dbContext;
        }

        [FunctionName("OperationsCreateWorkorder")]
        public void Run([TimerTrigger("0 */2 * * * *")]TimerInfo functionTimer, ILogger log)
        {
            var assetsList = _DbContext.AssetOperation.Where(p => p.WorkRequired == true && p.WorkorderCreated == false).ToList();
            foreach(var asset in assetsList)
            {
                var eventMessage = new EventMessage() { CorrelationId = Guid.NewGuid(), CreatedBy = $"{SERVICE_ID} Service", CreatedBySystem = $"{SERVICE_ID} Service",
                                        CreatedTime = DateTime.Now, EventType = EVENT_TYPE_ID, NotifyCompletion = false, Message = JsonConvert.SerializeObject(asset) };
                try
                {
                    if (SendMessageToServiceBus(eventMessage))
                    {
                        log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} service sent message to {Environment.GetEnvironmentVariable("OrchestrationServiceTopicId")} topic");
                        asset.WorkorderCreated = true;
                        _DbContext.SaveChanges();
                        log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} create workorder for asset {asset.AssetId} triggered");
                    }
                    else
                    {
                        log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} create workorder for asset {asset.AssetId} failed. Unknown Error");
                    }
                }
                catch(Exception ex)
                {
                    log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} create workorder for asset {asset.AssetId} failed. Exception: {ex.Message}");
                }
            }
        }

        private bool SendMessageToServiceBus(EventMessage eventMessage)
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
            return true;
        }
    }
}
