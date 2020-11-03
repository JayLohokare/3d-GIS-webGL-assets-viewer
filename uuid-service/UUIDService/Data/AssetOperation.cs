using System;

namespace UUIDService.Data
{
    public class AssetOperation
    {
        public int Id { get; set; }
        public Guid AssetId { get; set; }
        public string AssetStatus { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
        public bool WorkRequired { get; set; }
        public bool? WorkorderCreated { get; set; }
    }
}