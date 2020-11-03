using System;
using System.Collections.Generic;

namespace Common.Data.DTO
{
    internal class AssetRequest
    {
        public Guid AssetId;
        public string Status;
        public string Equipement;
        public string Type;
        public string Description;
        public string OperationalStatus;
        public string Class;
        public string ParentAsset;
        public string PrimarySystem;
        public string AssetTypeDescription;
        public int LayerId;
        public string FeatureClassName;
        public int? FeatureId;
        public string ModelGroup;
        public string ModelName;
    }
}