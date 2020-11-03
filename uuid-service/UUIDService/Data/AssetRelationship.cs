using System;

namespace UUIDService.Data
{
    public class AssetRelationship
    {
        public int Id { get; set; }
        public Guid AssetId { get; set; }
        public Guid RelatedAssetId { get; set; }
        public string RelationshipId { get; set; }
    }
}