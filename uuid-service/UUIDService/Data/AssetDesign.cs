using System;
using System.ComponentModel.DataAnnotations;

namespace UUIDService.Data
{
    public class AssetDesign
    {
        [Key]
        public Guid AssetId { get; set; }
        public string ModelId { get; set; }
        public string Material { get; set; }
        public string StructureType { get; set; }
        public string FeatureClassName { get; set; }
        public string ModelGroup { get; set; }
        public string ModelName { get; set; }
        public string ModelURL { get; set; }
        public int? FeatureId { get; set; }
    }
}