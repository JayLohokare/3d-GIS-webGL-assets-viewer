using System;
using System.ComponentModel.DataAnnotations;


namespace ModelService
{
    public class ModelAssetConfig
    {

        [Key]
        public int Id { get; set; }
        public string ModelName { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public float ZLocation { get; set; }
        public float Scale { get; set; }
        public Guid? UUID { get; set; }
        public int AssetGroup { get; set; }
        public int AssetType { get; set; }
    }
}
