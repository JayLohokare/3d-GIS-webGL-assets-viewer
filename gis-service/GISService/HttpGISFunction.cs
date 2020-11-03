using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Common.Data;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using GISService.Services;

namespace GISService
{
    public static class HttpGISFunction
    {
        public static readonly List<string> GetAllFeaturesInFeatureServersInViewportEvent = new List<string>() { "3D_GET_ASSETS_IN_VIEWPORT_GIS" };
        public static readonly string SERVICE_ID = "GIS";
        private static ArcGISService _arcGISService = new ArcGISService();

        [FunctionName("HttpGISFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "call/{action}")] HttpRequest req, string action, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //get the message
            var eventMessage = new EventMessage(requestBody);
            log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - GIS service recieved http {action} request");

            var responseDictionary = new Dictionary<string, JToken>();
            if (GetAllFeaturesInFeatureServersInViewportEvent.Contains(eventMessage.EventType.ToUpper()))
            {
                var request = JsonConvert.DeserializeObject<FeaturesRequest>(eventMessage.Message);
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

                log.LogInformation($"CorrelationId: {eventMessage.CorrelationId} - {SERVICE_ID} get features successful");
            }

            return new OkObjectResult(responseDictionary);
        }
    }
}
