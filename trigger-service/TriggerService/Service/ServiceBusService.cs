using Common.Data;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using TriggerService.Data;

namespace TriggerService.Service
{
    class ServiceBusService
    {
        static ITopicClient topicClient;
        public static CallReponseObject SendMessageToOrchestrationService(string message, string action)
        {
            string TopicName = Environment.GetEnvironmentVariable("OutboundServiceBusTriggerTopic");
            string ServiceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            var messageObject = new EventMessage(message);
            if (string.IsNullOrEmpty(messageObject.EventType))
                messageObject.EventType = action;
            var success = SendMessagesAsync(JsonConvert.SerializeObject(messageObject), messageObject.CorrelationId.Value.ToString()).Result;

            topicClient.CloseAsync();

            return new CallReponseObject() { Success = success, CorrelationId = messageObject.CorrelationId.Value.ToString() };
        }

        private static async Task<bool> SendMessagesAsync(string triggermessage, string CorrelationId)
        {
            try
            {
                var message = new Message
                {
                    ContentType = "application/json",
                    Body = Encoding.UTF8.GetBytes(triggermessage),
                    CorrelationId = CorrelationId,
                    Label = string.Format("AUTOUD From TRIGGER To ORCHESTRATION")
                };
                await topicClient.SendAsync(message);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
                return false;
            }
        }
    }
}
