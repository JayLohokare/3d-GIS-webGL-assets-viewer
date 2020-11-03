using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;

namespace OrchestrationService.Data
{
    public class OrchestrationDataContext : DbContext
    {
        private AzureServiceTokenProvider azureServiceTokenProvider;
        public OrchestrationDataContext(DbContextOptions<OrchestrationDataContext> options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
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
        public DbSet<EventServiceMapView> EventServiceMapView { get; set; }
    }
}