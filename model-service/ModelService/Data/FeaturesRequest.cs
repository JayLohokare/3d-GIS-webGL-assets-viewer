using System;
using System.Collections.Generic;

namespace Common.Data
{
    internal class FeaturesRequest : FeatureServerRequest
    {
        public List<Guid> AssetIds;
        public List<List<double>> Viewport;
    }
}