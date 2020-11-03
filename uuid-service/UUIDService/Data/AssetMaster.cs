using System;
using System.ComponentModel.DataAnnotations;

namespace UUIDService.Data
{
    public class AssetMaster
    {
        [Key]
        public Guid AssetId { get; set; } = Guid.NewGuid();
        public string Type { get; set; }
        public string Equipment { get; set; }
        public string Description { get; set; }
        public string LoanedToDepartment { get; set; }
        public string PmWoDepartment { get; set; }
        public string Organization { get; set; }
        public string OperationalStatus { get; set; }
        public string Status { get; set; }
        public string Class { get; set; }
        public string EquipmentConfiguration { get; set; }
        public string ParentAsset { get; set; }
        public string Dependent { get; set; }
        public string Position { get; set; }
        public string PositionParent { get; set; }
        public string PrimarySystem { get; set; }
        public string Location { get; set; }
        public string Manufacturer { get; set; }
        public string SerialNo { get; set; }
        public string Model { get; set; }
        public string Part { get; set; }
        public string Store { get; set; }
        public string Bin { get; set; }
        public string Lot { get; set; }
        public string OwnershipType { get; set; }
        public string AssetTypeKey { get; set; }
        public string AssetTypeDescription { get; set; }
        public string ModelCU { get; set; }
    }
}
