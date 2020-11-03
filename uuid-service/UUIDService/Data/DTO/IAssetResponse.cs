using System;
using System.Collections.Generic;
using UUIDService.Data;

namespace Common.Data.DTO
{
    internal interface IAssetResponse
    {
        Guid AssetId { get; set; }
    }
}