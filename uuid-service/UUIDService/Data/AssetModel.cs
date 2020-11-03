using System;
using System.ComponentModel.DataAnnotations;

namespace UUIDService.Data
{
    public class AssetModel
    {
        [Key]
        public string ModelId { get; set; }
        public string ModelGroup { get; set; }
        public string ModelName { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string PartFamilyId { get; set; }
        public string PartFamilyName { get; set; }
        public string AssetGroupId { get; set; }
        public string AssetType { get; set; }
    }
}