using Common.Data.DTO;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UUIDService.Data;

namespace UUIDService
{
    public class UUIDDataContext : DbContext
    {
        private AzureServiceTokenProvider azureServiceTokenProvider;

        public UUIDDataContext(DbContextOptions<UUIDDataContext> options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
        {
            this.azureServiceTokenProvider = azureServiceTokenProvider;
        }

        public DbSet<AssetDesign> AssetDesign { get; set; }
        public DbSet<AssetGeospatial> AssetGeospatial { get; set; }
        public DbSet<AssetHistory> AssetHistory { get; set; }
        public DbSet<AssetMaster> AssetMaster { get; set; }
        public DbSet<AssetModel> AssetModel { get; set; }
        public DbSet<AssetOperation> AssetOperation { get; set; }
        public DbSet<AssetPhysical> AssetPhysical { get; set; }
        public DbSet<AssetRelationship> AssetRelationship { get; set; }
        public DbSet<AssetRelationshipMaster> AssetRelationshipMaster { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            connection.AccessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net/").Result;
            optionsBuilder.UseSqlServer(connection);
        }

        internal AssetResponse GetAssetResponseById(Guid assetId)
        {
            var response = new AssetResponse() { AssetId = assetId };
            response.Master = AssetMaster.SingleOrDefault(b => b.AssetId == assetId);
            response.Physical = AssetPhysical.SingleOrDefault(b => b.AssetId == assetId);
            response.Design = AssetDesign.SingleOrDefault(b => b.AssetId == assetId);
            response.Geospatial = AssetGeospatial.SingleOrDefault(b => b.AssetId == assetId);
            response.Operation = AssetOperation.SingleOrDefault(b => b.AssetId == assetId);
            response.History = AssetHistory.Where(b => b.AssetId == assetId).ToList();
            response.Relationships = AssetRelationship.Where(b => b.AssetId == assetId).ToList();
            return response;
        }

        internal Guid AddNewAsset(AssetRequest request)
        {
            var assetId = Guid.NewGuid();
            var changeDate = DateTime.Now;

            var assetMaster = new AssetMaster()
            {
                AssetId = assetId,
                Status = request.Status,
                Equipment = request.Equipement,
                Description = request.Description,
                Type = request.Type,
                OperationalStatus = request.OperationalStatus,
                PrimarySystem = request.PrimarySystem,
                Class = request.Class,
                ParentAsset = request.ParentAsset,
                AssetTypeDescription = request.AssetTypeDescription
            };
            AssetMaster.Add(assetMaster);

            AssetPhysical.Add(new AssetPhysical() { AssetId = assetId });
            AssetDesign.Add(new AssetDesign() { AssetId = assetId, FeatureClassName = request.FeatureClassName, FeatureId = request.FeatureId, ModelGroup = request.ModelGroup, ModelName = request.ModelName });
            AssetOperation.Add(new AssetOperation() { AssetId = assetId, WorkRequired = false, ChangeDate = changeDate });
            AssetGeospatial.Add(new AssetGeospatial() { AssetId = assetId, GisId = assetId, LayerId = request.LayerId });
            AssetHistory.Add(new AssetHistory() { AssetId = assetId, AssetStatus = request.Status, StatusChangeDate = changeDate });
            SaveChanges();
            return assetId;
        }

        internal List<Guid> GetAssetGeospacialInViewPort(List< System.Single> viewport){
            if (viewport.Count != 4)
            {
                //Return empty list when Invalid viewport OR All assets match viewport criteria
                return new List<Guid>();
            }
            var filteredAssets = AssetGeospatial.Where(b => (b.XCoordinate >= viewport[0]) && (b.YCoordinate >= viewport[1]) && (b.XCoordinate <= viewport[2]) && (b.YCoordinate    <= viewport[3]) ).ToList();
            var geospatialFilteredAssetIds = filteredAssets.Select(asset => asset.AssetId).ToList();

            if(geospatialFilteredAssetIds.Count == 0)
            {
                //Return null when no assets match criteria
                return null;
            }
            return geospatialFilteredAssetIds;
        }

        internal List<IAssetResponse> QueryAssets(AssetQueryRequest request)
        {
            if (request.Date == null || request.Date == DateTime.MinValue)
            {
                request.Date = DateTime.Now;
            }

            if(request.Status == null)
            {
                request.Status = "All";
            }

            List<AssetHistory> filteredAssets;

            //Filter assets by GIS layer
            if (request.LayerId != 0)
            {
                var assetIds = AssetGeospatial.Where(p => p.LayerId == request.LayerId).Select(p => p.AssetId).ToList();
                filteredAssets = AssetHistory.Where(p => assetIds.Contains(p.AssetId)).ToList();
            }
            else
                filteredAssets = AssetHistory.ToList();

            // filter over list of Asset IDs
            if (request.AssetIds.Count > 0)
            {
                filteredAssets = filteredAssets.Where(p => request.AssetIds.Contains(p.AssetId)).ToList();
            }

            // apply search
            if(!string.IsNullOrEmpty(request.Search))
            {
                var searchedAssetsIds =  AssetMaster.Where(p => p.Description.Contains(request.Search)).Select(p => p.AssetId).ToList();
                filteredAssets = filteredAssets.Where(p => searchedAssetsIds.Contains(p.AssetId)).ToList();
            }

            //Find latest entry corresponding to all assets as per date entered
            filteredAssets = filteredAssets.FindAll(x => x.StatusChangeDate <= request.Date);
            filteredAssets = filteredAssets
                .GroupBy(x => x.AssetId)
                .Select(y => y.OrderByDescending(z => z.StatusChangeDate).First()).ToList();

            //Filter asset entries by status
            if (request.Status != "All" && request.Status != "" && request.Status != null)
            {
                filteredAssets = filteredAssets.FindAll(x => x.AssetStatus == request.Status);
            }

            var assetsList = new List<IAssetResponse>();
            if (request.ReturnBasicAttributes)
            {
                var assetIds = filteredAssets.Select(p => p.AssetId).ToList();
                var filteredMasterAssets = AssetMaster.Where(p => assetIds.Contains(p.AssetId)).ToList();
                foreach (var asset in filteredMasterAssets)
                {
                    assetsList.Add(new AssetBasicResponse() { AssetId = asset.AssetId, Description = asset.Description, Equipement = asset.Equipment, Status = asset.Status, Type = asset.Type });
                }
                return assetsList;
            }
            else
            {
                foreach (var asset in filteredAssets)
                {
                    Guid assetId = asset.AssetId;
                    assetsList.Add(GetAssetResponseById(assetId));
                }
                return assetsList;
            }
        }
    }
}