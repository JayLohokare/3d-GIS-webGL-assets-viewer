using System;

namespace UUIDService.Data
{
    public class AssetHistory
    {
        public int Id { get; set; }
        public Guid AssetId { get; set; }
        public DateTime StatusChangeDate { get; set; } = DateTime.UtcNow;
        public string AssetOwner { get; set; }
        public string AsseType { get; set; }
        public string AssetStatus { get; set; }
    }
}