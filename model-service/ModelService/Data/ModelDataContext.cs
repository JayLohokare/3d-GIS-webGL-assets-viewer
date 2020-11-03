using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace ModelService
{
    public class ModelDataContext : DbContext
    {
        private AzureServiceTokenProvider azureServiceTokenProvider;
        public ModelDataContext(DbContextOptions<ModelDataContext> options, AzureServiceTokenProvider azureServiceTokenProvider) : base(options)
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
        public DbSet<Model> Model { get; set; }
        public DbSet<ModelAssetConfig> ModelAssetConfig { get; set; }
    }
}