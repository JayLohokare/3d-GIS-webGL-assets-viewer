using Common.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TriggerService.Service;

namespace TriggerService
{
    public static class SynchronousCall
    {
        [FunctionName("SynchronousCall")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "call/{Action}")] HttpRequest req, string Action,
            ILogger log)
        {
            string triggermessage = await new StreamReader(req.Body).ReadToEndAsync();
            var response = ServiceBusService.SendMessageToOrchestrationService(triggermessage, Action);
            int synchronousCallTimeLimitToRespondInSeconds = 60;
            int.TryParse(Environment.GetEnvironmentVariable("SynchronousCallTimeLimitToRespondInSeconds"), out synchronousCallTimeLimitToRespondInSeconds);
            if (response.Success)
            {
                log.LogInformation($"CorrelationId: {response.CorrelationId} - Trigger service sent message to orchestration");
                var dateTimeToStopRetying = DateTime.Now.AddSeconds(synchronousCallTimeLimitToRespondInSeconds);
                var messageReceiver = new MessageReceiver(
                    new ServiceBusConnectionStringBuilder(Environment.GetEnvironmentVariable("ServiceBusEndPoint"),
                                                            string.Format("{0}/subscriptions/{1}", Environment.GetEnvironmentVariable("InboundServiceBusTriggerTopic"), Environment.GetEnvironmentVariable("InboundServiceBusTriggerSubscription")),
                                                            Environment.GetEnvironmentVariable("ServiceBusSharedAccessKeyName"),
                                                            Environment.GetEnvironmentVariable("ServiceBusSharedAccessKey")),
                    ReceiveMode.PeekLock,
                    RetryPolicy.Default);

                var message = messageReceiver.PeekAsync().Result;
                var lastPeekedSequenceNumber = messageReceiver.LastPeekedSequenceNumber;
                while (true)
                {
                    if (DateTime.Now > dateTimeToStopRetying)
                        break;

                    if (message != null)
                    {
                        if (message.CorrelationId == response.CorrelationId)
                        {
                            var messageObject = new EventMessage(Encoding.Default.GetString(message.Body));

                            // receive the message and complete
                            var messageToComplete =  messageReceiver.ReceiveAsync().Result;
                            await messageReceiver.CompleteAsync(messageToComplete.SystemProperties.LockToken);
                            log.LogInformation($"CorrelationId: {message.CorrelationId} - Trigger service synchronous call returned response");
                            await messageReceiver.CloseAsync();

                            //send response
                            return new OkObjectResult(messageObject);
                        }
                    }
                    message = messageReceiver.PeekBySequenceNumberAsync(messageReceiver.LastPeekedSequenceNumber + 1).Result;
                    if (lastPeekedSequenceNumber == messageReceiver.LastPeekedSequenceNumber)
                        message = messageReceiver.PeekBySequenceNumberAsync(1).Result;
                    lastPeekedSequenceNumber = messageReceiver.LastPeekedSequenceNumber;
                }
            }

            string errorMessage = $"CorrelationId: {response.CorrelationId} - Trigger service failed to send message to orchestration";
            log.LogError(errorMessage);
            return new BadRequestObjectResult(errorMessage);
        }
    }
}
