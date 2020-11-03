using System;
using System.Collections.Generic;

namespace Common.Data
{
    internal class AssetRequest
    {
        public List<string> AssetIds;
        public string Status;
        public DateTime Date;
        public int LayerId;
        public string Search;
        public bool ReturnBasicAttributes;
    }
}