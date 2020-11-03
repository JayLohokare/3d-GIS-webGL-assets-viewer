using System;

namespace Common.Data
{
    internal class FeatureServerRequest
    {
        private readonly string Electric_Utility_Network_Path = "Electric_Utility_Network";
        //private readonly string AUD_Design_Layers_Path = "Hosted/AUD_Design_Layers";
        private readonly string AUD_Design_Layers_Path = "Hosted/Design_Layers_UUID";

        public int? LayerId;
        public int? MaxCount;
        public bool IsStaged;
        public string FeatureServerPath
        {
            get
            {
                if (IsStaged)
                    return AUD_Design_Layers_Path;
                return Electric_Utility_Network_Path;
            }
        }
        public string FeatureServerFields
        {
            get
            {
                if (IsStaged)
                    return "*";
                return "GLOBALID,SUBNETWORKNAME,OBJECTID,ASSETGROUP,ASSETTYPE,ISCONNECTED,lifecyclestatus";
            }
        }
    }
}