using System;

namespace Common.Data
{
    internal class AssetEamResponse
    {
        public Guid AssetId;
        public string Make;
        public string Model;
        public string SerialNumber;
        public string AssetType;
        public double Longitude;
        public double Latitude;
    }
}