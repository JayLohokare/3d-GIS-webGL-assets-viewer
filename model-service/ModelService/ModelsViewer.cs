using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Common.Data;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;
using ModelService;

namespace ModelViewer
{
    public class ModelsViewer
    {
        static string TriggerServiceURL = Environment.GetEnvironmentVariable("TriggerServiceURL");
        static string GISServiceURL = Environment.GetEnvironmentVariable("GISServiceURL");
        static string SERVICE_ID = "3D";
        static string EVENT_3D_GET_FEATURE_SERVERS_GIS = "3D_GET_FEATURE_SERVERS_GIS";
        static string EVENT_3D_GET_ASSETS_IN_VIEWPORT_GIS = "3D_GET_ASSETS_IN_VIEWPORT_GIS";
        static string EVENT_3D_SEARCH_ASSETS_REGISTRY = "3D_SEARCH_ASSETS_REGISTRY";

        private static ILogger _log;
        private readonly ModelDataContext _DbContext;
        public ModelsViewer(ModelDataContext dbContext)
        {
            _DbContext = dbContext;
        }

        [FunctionName("ModelsViewer")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "getAssets/")] HttpRequest req, ILogger log)
        {
            _log = log;

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var assetRequest = JsonConvert.DeserializeObject<AssetRequestPayload>(requestBody);
            if (string.IsNullOrEmpty(assetRequest.Status))
                assetRequest.Status = "All";
            if (!assetRequest.Date.HasValue)
                assetRequest.Date = DateTime.Now;

            var stagedLayers = assetRequest.StagedLayers;
            if (stagedLayers == null || stagedLayers.Count == 0)
                stagedLayers = GetFeatureLayersInViewportAsync(true);

            var networkLayers = assetRequest.NetworkLayers;
            if (networkLayers == null || networkLayers.Count == 0)
                networkLayers = GetFeatureLayersInViewportAsync(false);

            //get assets from ArcGIS
            var stagedAssets = GetAssetsInLayer(assetRequest, true, stagedLayers);
            var nonStagedAssets = GetAssetsInLayer(assetRequest, false, networkLayers);
            var assetsInViewport = stagedAssets.Union(nonStagedAssets).ToDictionary(pair => pair.Key, pair => pair.Value);
            log.LogInformation($"ModelsViewer returned {assetsInViewport.Keys.Count()} assets from AcrGIS");

            //get assets from Registry
            var assets = GetAssetsWithStatus(assetRequest.Status, assetRequest.Date.Value, assetRequest.Search, assetsInViewport.Keys.ToList());
            var assetList = new List<Guid>();
            var assetGroupList = new List<int>();
            var assetTypeList = new List<int>();
            foreach (var asset in assets)
            {
                var assetId = asset["AssetId"].ToString().ToUpper();
                assetList.Add(Guid.Parse(assetId));
                if (assetsInViewport.Keys.Contains(assetId))
                {
                    var assetResonse = assetsInViewport[assetId];
                    assetGroupList.Add(assetResonse.AssetGroup);
                    assetTypeList.Add(assetResonse.AssetType);
                }
            }
            //query records from DB
            var _modelAssetConfigByUUIDList = _DbContext.ModelAssetConfig.Where(p => assetList.Contains(p.UUID.Value)).ToList();
            var _modelAssetConfigByGroupTypeList = _DbContext.ModelAssetConfig.Where(p => assetGroupList.Contains(p.AssetGroup) && assetTypeList.Contains(p.AssetType)).ToList();
            
            //loop over assets and find proper config record
            foreach (var asset in assets)
            {
                var assetId = asset["AssetId"].ToString().ToUpper();
                if (assetsInViewport.Keys.Contains(assetId))
                {
                    var assetResonse = assetsInViewport[assetId];
                    var configRecord = _modelAssetConfigByUUIDList.Where(p => p.UUID == Guid.Parse(assetId)).FirstOrDefault();
                    if (configRecord == null)
                    {
                        configRecord = _modelAssetConfigByGroupTypeList.Where(p => p.AssetGroup == assetResonse.AssetGroup && p.AssetType == assetResonse.AssetType).FirstOrDefault();
                    }

                    //update response from config
                    if (configRecord != null)
                    {
                        assetResonse.ModelName = configRecord.ModelName;
                        assetResonse.Z = configRecord.ZLocation;
                        assetResonse.Scale = configRecord.Scale;
                        assetResonse.RotationX = configRecord.RotationX;
                        assetResonse.RotationY = configRecord.RotationY;
                        assetResonse.RotationZ = configRecord.RotationZ;
                    }
                    else
                        log.LogWarning($"No ModelAssetConfig for asset: {assetId}. AssetGroup: {assetResonse.AssetGroup}, AssetType {assetResonse.AssetType}");

                    asset["geometry"] = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(assetResonse));
                }
            }

            log.LogInformation($"ModelsViewer returned {assets.Count()} assets");
            return new OkObjectResult(assets);
        }

        public static List<int> GetFeatureLayersInViewportAsync(bool isStaged)
        {
            var layerIds = new List<int>();

            var eventMessage = new EventMessage() { CreatedBySystem = SERVICE_ID, EventType = EVENT_3D_GET_FEATURE_SERVERS_GIS, CreatedBy = SERVICE_ID, CreatedTime = DateTime.Now };
            eventMessage.Message = JsonConvert.SerializeObject(new FeatureServerRequest() { IsStaged = isStaged });
            
            var client = new RestClient(TriggerServiceURL);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(eventMessage), ParameterType.RequestBody);

            var response = client.Execute(request);
            var layers = JsonConvert.DeserializeObject<JToken>(response.Content)["message"]["layers"];
            foreach(var layer in layers)
            {
                layerIds.Add(int.Parse(layer["id"].ToString()));
            }
            return layerIds;
        }

        public static Dictionary<string, AssetResponse> GetAssetsInLayer(AssetRequestPayload assetRequest, bool isStaged, List<int> layerIds)
        {
            Dictionary<string, AssetResponse> assetObjects = new Dictionary<string, AssetResponse>();

            foreach (int layerId in layerIds) // Loop through List with foreach
            {
                var eventMessage = new EventMessage() { CreatedBySystem = SERVICE_ID, EventType = EVENT_3D_GET_ASSETS_IN_VIEWPORT_GIS, CreatedBy = SERVICE_ID, CreatedTime = DateTime.Now };
                eventMessage.Message = JsonConvert.SerializeObject(new FeaturesRequest() { IsStaged = isStaged, LayerId = layerId, MaxCount = -1, Viewport = assetRequest.Viewport });
                var client = new RestClient(GISServiceURL);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(eventMessage), ParameterType.RequestBody);

                var response = client.Execute(request);
                var features = JsonConvert.DeserializeObject<JToken>(response.Content)[layerId.ToString()]["features"];
                if(features != null)
                { 
                    foreach (var feature in features)
                    {
                        var uniqueKey = "";
                        if (feature["attributes"]["uuid"] != null) { uniqueKey = "uuid"; }
                        else if (feature["attributes"]["GLOBALID"] != null) { uniqueKey = "GLOBALID"; }
                        else if (feature["attributes"]["globalid"] != null) { uniqueKey = "globalid"; }

                        var assettypeAttribute = isStaged ? "assettype" : "ASSETTYPE";
                        var assetgroupAttribute = isStaged ? "assetgroup" : "ASSETGROUP";

                        var assetId = feature["attributes"][uniqueKey].ToString().ToUpper();
                        if (assetId.StartsWith('{') && assetId.EndsWith('}'))
                            assetId = assetId.Substring(1, assetId.ToString().Length - 2);

                        var assetType = int.Parse(feature["attributes"][assettypeAttribute].ToString());
                        var assetGroup = int.Parse(feature["attributes"][assetgroupAttribute].ToString());

                        if (assetType == 0 || assetGroup == 0)
                        {
                            _log.LogWarning($"Ignoring assetId {assetId} in layer {layerId}. Asset type: {assetType}, Asset group: {assetGroup}");
                            break;
                        }

                        if (feature["geometry"] == null || feature["geometry"]["x"] == null || feature["geometry"]["y"] == null)
                        {
                            _log.LogWarning($"Ignoring assetId {assetId} in layer {layerId}. Asset geometry invalid");
                            break;
                        }

                        if (string.IsNullOrEmpty(assetId))
                        {
                            _log.LogWarning($"Ignoring null or empty assetId in layer {layerId}. Asset type: {assetType}, Asset group: {assetGroup}");
                            break;
                        }
                        
                        if (assetObjects.ContainsKey(assetId))
                        {
                            _log.LogWarning($"Ignoring assetId {assetId} in layer {layerId}. Found same assetId in {assetObjects[assetId].LayerId}");
                            break;
                        }
                        
                        assetObjects.Add(assetId, new AssetResponse() { AssetType = assetType, AssetGroup = assetGroup, LayerId = layerId });
                        if (feature["geometry"]["x"] != null && feature["geometry"]["y"] != null)
                        {
                            assetObjects[assetId].X = double.Parse(feature["geometry"]["x"].ToString());
                            assetObjects[assetId].Y = double.Parse(feature["geometry"]["y"].ToString());
                        }
                    }
                }
            }
            return assetObjects;
        }

        public static JToken GetAssetsWithStatus(string status, DateTime datetime, string searchText, List<string> assetIds)
        {
            var eventMessage = new EventMessage() { CreatedBySystem = SERVICE_ID, EventType = EVENT_3D_SEARCH_ASSETS_REGISTRY, CreatedBy = SERVICE_ID, CreatedTime = DateTime.Now };
            eventMessage.Message = JsonConvert.SerializeObject(new AssetRequest() { Status = status, Date = datetime, Search = searchText, AssetIds = assetIds, ReturnBasicAttributes = true });
            var client = new RestClient(TriggerServiceURL);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(eventMessage), ParameterType.RequestBody);

            var response = client.Execute(request);

            return JsonConvert.DeserializeObject<JToken>(response.Content)["message"];
        }

    }
}
