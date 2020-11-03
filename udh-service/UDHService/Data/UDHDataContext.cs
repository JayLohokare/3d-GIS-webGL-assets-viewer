using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace UDHService.Data
{
    public class UDHDataContext : DbContext
    {
        private AzureServiceTokenProvider azureServiceTokenProvider;
        public UDHDataContext(DbContextOptions<UDHDataContext> options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
        {
            this.azureServiceTokenProvider = azureServiceTokenProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            connection.AccessToken = azureServiceTokenProvider.GetAccessTokenAsync("https://database.windows.net/").Result;
            optionsBuilder.UseSqlServer(connection);
        }
        public DbSet<WorkRequest> WorkRequest { get; set; }

        public bool AddOrUpdateWorkorder(WorkRequest workRequest)
        {
            var recordExists = WorkRequest.FirstOrDefault(p => p.WorkorderNumber.ToLower() == workRequest.WorkorderNumber.ToLower());
            if (recordExists != null)
            {
                recordExists.WorkRequestStatus = workRequest.WorkRequestStatus;
                recordExists.WorkRequestOwnerName = workRequest.WorkRequestOwnerName;
                recordExists.WorkRequestType = workRequest.WorkRequestType;
                recordExists.WorkRequestDescription = workRequest.WorkRequestDescription;
            }
            else
            {
                WorkRequest.Add(workRequest);
            }
            try
            {
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}