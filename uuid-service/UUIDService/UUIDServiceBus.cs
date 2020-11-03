using System;
using System.Text;
using Common.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Linq;
using Common.Data.DTO;
using UUIDService.Data;
using Newtonsoft.Json.Linq;

namespace UUIDService
{
    public class UUIDServiceBus
    {
        public static readonly string SERVICE_ID = "REGISTRY";
        public static readonly List<string> GetAssetEvents = new List<string>() { "3D_GET_ASSET_EAM" };
        public static readonly List<string> UpdateAssetEvents = new List<string>() { "EAM_UPDATE_ASSET_GIS" };
        public static readonly List<string> AddAssetEvents = new List<string>() { "UDH_ASSET_CREATE_REGISTRY" };
        public static readonly List<string> AddAssetsEvents = new List<string>() { "UDH_ASSETS_CREATE_REGISTRY" };
        public static readonly List<string> GetAssetsInViewportEvent = new List<string>() { "3D_GET_ASSETS_IN_VIEWPORT" };
        public static readonly List<string> SearchAssetsEvent = new List<string>() { "3D_SEARCH_ASSETS_REGISTRY" };
        public static readonly string AssetStatusChangedEvent = "REGISTRY_ASSET_STATUS_CHANGED";

        private readonly UUIDDataContext _DbContext;
        public UUIDServiceBus(UUIDDataContext dbContext)
        {
            _DbContext = dbContext;
        }
        
        [FunctionName("UUIDServiceBus")]
        public void Run([ServiceBusTrigger("AUTOUD_REGISTRY", "AUTOUD_REGISTRY", Connection = "ServiceBusConnectionString")]string serviceBusMessage, ILogger log)
        {
            //get the message
            var eventMessage = new EventMessage(serviceBusMessage);
            log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} service recieved message");

            //handle the message
            //GET Asset data
            if (GetAssetEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetRequest>(eventMessage.Message);
                eventMessage.Message = JsonConvert.SerializeObject(_DbContext.GetAssetResponseById(request.AssetId));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get asset {request.AssetId} successful");
            }
            //Get Assets in viewport
            else if (GetAssetsInViewportEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetQueryRequest>(eventMessage.Message);
                var messageToken = JsonConvert.DeserializeObject<JToken>(eventMessage.Message);
                messageToken["registry"] = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(_DbContext.QueryAssets(request)));
                eventMessage.Message = JsonConvert.SerializeObject(messageToken);
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} got {eventMessage.Message.Count()} assets in viewport successful");
            }
            //Search Assets 
            else if (SearchAssetsEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetQueryRequest>(eventMessage.Message);
                eventMessage.Message = JsonConvert.SerializeObject(_DbContext.QueryAssets(request));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} got assets from search successful");
            }
            //Update Asset data
            else if (UpdateAssetEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetRequest>(eventMessage.Message);
                var assetMaster = _DbContext.AssetMaster.SingleOrDefault(b => b.AssetId == request.AssetId);
                if(assetMaster.Status.ToLower() != request.Status.ToLower())
                {
                    // Send REGISTRY_ASSET_STATUS_CHANGED event if asset status changed
                    _DbContext.AssetHistory.Add(new AssetHistory() { AssetId = assetMaster.AssetId, AssetStatus = request.Status, StatusChangeDate = DateTime.Now });
                    var statusChangeEvent = new EventMessage() { CreatedBySystem = SERVICE_ID, CreatedBy = SERVICE_ID, CorrelationId = Guid.NewGuid(), CreatedTime = DateTime.Now
                                                            , EventType = AssetStatusChangedEvent, Message = eventMessage.Message = JsonConvert.SerializeObject(assetMaster) };

                    SendEventToServiceBus(statusChangeEvent, log);
                    log.LogInformation($"CorrelationId: {statusChangeEvent.CorrelationId} - {SERVICE_ID} sent status change event for asset {request.AssetId} successful. Original event Id: {eventMessage.CorrelationId}");
                }
                assetMaster.Status = request.Status;
                _DbContext.SaveChanges();
                eventMessage.Message = JsonConvert.SerializeObject(_DbContext.GetAssetResponseById(request.AssetId));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} update asset {request.AssetId} successful");
            }
            //Add Asset data
            else if (AddAssetEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetRequest>(eventMessage.Message);
                var assetId = _DbContext.AddNewAsset(request);
                eventMessage.Message = JsonConvert.SerializeObject(_DbContext.GetAssetResponseById(assetId));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} add asset {assetId} successful");
            }
            //Add Multiple Assets data
            else if (AddAssetsEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                List<AssetResponse> responses = new List<AssetResponse>();
                var request = JsonConvert.DeserializeObject<List<AssetRequest>>(eventMessage.Message);
                foreach(var assetRequest in request)
                {
                    var assetId = _DbContext.AddNewAsset(assetRequest);
                    responses.Add(_DbContext.GetAssetResponseById(assetId));
                }
                eventMessage.Message = JsonConvert.SerializeObject(responses);
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} added {request.Count} assets successfully");
            }

            //Send back response to Orchastration if needed
            if (eventMessage.NotifyCompletion)
            {
                SendEventToServiceBus(eventMessage, log);
            }
        }

        private void SendEventToServiceBus(EventMessage eventMessage, ILogger log)
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
            log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} service sent message to {Environment.GetEnvironmentVariable("OrchestrationServiceTopicId")} topic");
        }
    }
}
