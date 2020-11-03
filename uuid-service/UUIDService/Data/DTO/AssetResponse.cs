using System;
using System.Collections.Generic;
using UUIDService.Data;

namespace Common.Data.DTO
{
    internal class AssetResponse: IAssetResponse
    {
        public Guid AssetId { get; set; }
        public AssetMaster Master;
        public AssetPhysical Physical;
        public AssetDesign Design;
        public AssetOperation Operation;
        public AssetGeospatial Geospatial;
        public List<AssetRelationship> Relationships; 
        public List<AssetHistory> History;

    }
}