using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Common.Data
{
    public class AssetRequestPayload
    {
        public List<List<double>> Viewport;
        public string Status;
        public DateTime? Date;
        public string Search;
        public List<int> StagedLayers;
        public List<int> NetworkLayers;
    }
}
