using System;

namespace Common.Data
{
    internal class AssetRequest : FeatureServerRequest
    {
        public Guid AssetId;
        public string Status;
        public int GISObjectId;
    }
}