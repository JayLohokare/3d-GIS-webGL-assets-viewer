using System;
using System.ComponentModel.DataAnnotations;

namespace UUIDService.Data
{
    public class AssetRelationshipMaster
    {
        [Key]
        public string RelationshipId { get; set; }
        public string RelationshipType { get; set; }
        public string RelationshipDescription { get; set; }
    }
}