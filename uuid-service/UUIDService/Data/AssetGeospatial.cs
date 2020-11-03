using System;
using System.ComponentModel.DataAnnotations;

namespace UUIDService.Data
{
    public class AssetGeospatial
    {
        [Key]
        public Guid GisId { get; set; }
        public Guid AssetId { get; set; }
        public string Map { get; set; }
        public string MapOrg { get; set; }
        public string Layer { get; set; }
        public int LayerId { get; set; }
        public System.Single XCoordinate { get; set; }
        public System.Single YCoordinate { get; set; }
        public System.Single ZCoordinate { get; set; }
        public System.Single XRotation { get; set; }
        public System.Single YRotation { get; set; }
        public System.Single ZRotation { get; set; }
        public System.Single Scale { get; set; }
    }
}