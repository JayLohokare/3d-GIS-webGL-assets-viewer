using System;

namespace Common.Data.DTO
{
    internal class AssetBasicResponse: IAssetResponse
    {
        public Guid AssetId { get; set; }
        public string Status;
        public string Equipement;
        public string Type;
        public string Description;
    }
}