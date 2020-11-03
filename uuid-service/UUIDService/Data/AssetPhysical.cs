using System;
using System.ComponentModel.DataAnnotations;

namespace UUIDService.Data
{
    public class AssetPhysical
    {
        [Key]
        public Guid AssetId { get; set; }
        public string EquipmentLength { get; set; }
        public string EquipmentLengthUom { get; set; }
        public string LinearReferenceUom { get; set; }
        public string ReferencePrecision { get; set; }
        public string GgeographicalReference { get; set; }
        public string InspectionDirection { get; set; }
        public string LinearEquipmentType { get; set; }
        public string Direction { get; set; }
        public string HardwareVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public string OemSiteSystemId { get; set; }
        public string Vendor { get; set; }
    }
}