using System;
using System.Text;
using Common.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Collections.Generic;
using GISService.Services;
using Newtonsoft.Json.Linq;

namespace GISService
{
    public static class GISFunction
    {
        public static readonly string SERVICE_ID = "GIS";
        public static readonly List<string> GetAssetEvents = new List<string>() { "3D_GET_ASSET_GIS" };
        public static readonly List<string> UpdateAssetEvents = new List<string>() { "EAM_UPDATE_ASSET_GIS", "REGISTRY_ASSET_STATUS_CHANGED" };
        public static readonly List<string> TraceAssetEvents = new List<string>() { "3D_GET_ASSET_TRACE_GIS" };
        public static readonly List<string> GetFeatureServersEvent = new List<string>() { "3D_GET_FEATURE_SERVERS_GIS" };
        public static readonly List<string> GetFeaturesEvent = new List<string>() { "3D_GET_FEATURES_GIS" };
        public static readonly List<string> GetFeaturesInFeatureServersInViewportEvent = new List<string>() { "3D_GET_ASSETS_IN_VIEWPORT" };
        public static readonly List<string> GetAllFeaturesInFeatureServersInViewportEvent = new List<string>() { "3D_GET_ASSETS_IN_VIEWPORT_GIS" };
        public static readonly List<string> GetFeatureServerSubnetworksEvent = new List<string>() { "3D_GET_FEATURE_SERVER_SUBNETWORKS_GIS" };
        private static ArcGISService _arcGISService = new ArcGISService();

        [FunctionName("GISFunction")]
        public static void Run([ServiceBusTrigger("AUTOUD_GIS", "AUTOUD_GIS", Connection = "ServiceBusConnectionString")]string serviceBusMessage, ILogger log)
        {
            //get the message
            var eventMessage = new EventMessage(serviceBusMessage);
            log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - GIS service recieved message");

            //handle the message
            //Get Asset data
            if (GetAssetEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetRequest>(eventMessage.Message);
                var response = _arcGISService.GetAssetData(request);
                eventMessage.Message = JsonConvert.SerializeObject(JToken.Parse(response));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get asset {request.GISObjectId} successful");
            }
            //Update Asset data
            else if (UpdateAssetEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<AssetRequest>(eventMessage.Message);
                var response = _arcGISService.UpdateAsset(request);
                eventMessage.Message = JsonConvert.SerializeObject(JToken.Parse(response));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} update asset {request.AssetId} successful");
            }
            //Get Asset trace data
            else if (TraceAssetEvents.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<TraceRequest>(eventMessage.Message);
                var response = _arcGISService.GetAssetTrace(request);
                eventMessage.Message = JsonConvert.SerializeObject(JToken.Parse(response));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get asset trace for subnetwork {request.SubnetworkName} successful");
            }
            else if (GetFeatureServersEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<FeatureServerRequest>(eventMessage.Message);
                var response = _arcGISService.GetFeatureServerList(request);
                eventMessage.Message = JsonConvert.SerializeObject(JToken.Parse(response));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get feature servers list successful");
            }
            else if (GetFeatureServerSubnetworksEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<FeatureServerRequest>(eventMessage.Message);
                var response = _arcGISService.GetFeatureServerSubnetworks(request);
                eventMessage.Message = JsonConvert.SerializeObject(JToken.Parse(response));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get feature server subnetworks successful");
            }
            else if (GetFeaturesEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<FeaturesRequest>(eventMessage.Message);
                var response = _arcGISService.GetFeatures(request);
                eventMessage.Message = JsonConvert.SerializeObject(JToken.Parse(response));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get features successful");
            }
            else if (GetFeaturesInFeatureServersInViewportEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<FeaturesRequest>(eventMessage.Message);
                var responseDictionary = new Dictionary<string, JToken>();
                if (request.LayerId.HasValue)
                {
                    var response = _arcGISService.GetFeatures(request);
                    responseDictionary[request.LayerId.ToString()] = JsonConvert.DeserializeObject<JToken>(response);
                }
                else
                {
                    var featureServerList = JsonConvert.DeserializeObject<JToken>(_arcGISService.GetFeatureServerList(request));
                    foreach (var token in featureServerList["layers"])
                    {
                        var layerId = token["id"].ToString();
                        request.LayerId = int.Parse(layerId);
                        var response = _arcGISService.GetFeatures(request);
                        responseDictionary[layerId] = JsonConvert.DeserializeObject<JToken>(response);
                    }
                }
                var messageToken = JsonConvert.DeserializeObject<JToken>(eventMessage.Message);
                messageToken["gis"] = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(responseDictionary));
                
                eventMessage.Message = JsonConvert.SerializeObject(messageToken);
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get features successful");
            }
            else if (GetAllFeaturesInFeatureServersInViewportEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<FeaturesRequest>(eventMessage.Message);
                var responseDictionary = new Dictionary<string, JToken>();
                if (request.LayerId.HasValue)
                {
                    var response = _arcGISService.GetFeatures(request);
                    responseDictionary[request.LayerId.ToString()] = JsonConvert.DeserializeObject<JToken>(response);
                }
                else
                {
                    var featureServerList = JsonConvert.DeserializeObject<JToken>(_arcGISService.GetFeatureServerList(request));
                    foreach (var token in featureServerList["layers"])
                    {
                        var layerId = token["id"].ToString();
                        request.LayerId = int.Parse(layerId);
                        var response = _arcGISService.GetFeatures(request);
                        responseDictionary[layerId] = JsonConvert.DeserializeObject<JToken>(response);
                    }
                }

                eventMessage.Message = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(responseDictionary)));
                eventMessage.CreatedBySystem = SERVICE_ID;
                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get features successful");
            }

            //Send back response to Orchastration if needed
            if (eventMessage.NotifyCompletion)
            {
                var topicClient = new Microsoft.Azure.ServiceBus.TopicClient(Environment.GetEnvironmentVariable("ServiceBusConnectionString"), Environment.GetEnvironmentVariable("OrchestrationServiceTopicId"));
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
