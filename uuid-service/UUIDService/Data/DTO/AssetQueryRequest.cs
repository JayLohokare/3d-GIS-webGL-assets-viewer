using System;
using System.Collections.Generic;

namespace Common.Data.DTO
{
    internal class AssetQueryRequest
    {
        public List<Guid> AssetIds;
        public string Status;
        public DateTime Date;
        public int LayerId;
        public string Search;
        public bool ReturnBasicAttributes;
    }
}