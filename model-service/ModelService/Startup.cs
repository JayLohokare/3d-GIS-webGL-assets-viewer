using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelService.Services;
using System;

[assembly: FunctionsStartup(typeof(ModelService.Startup))]

namespace ModelService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(new AzureServiceTokenProvider());
            builder.Services.AddDbContext<ModelDataContext>();

            string azureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            string azureStorageContainerName = Environment.GetEnvironmentVariable("AzureStorageContainerName");
            builder.Services.AddSingleton<IStorageSevice>(new StorageService(azureStorageConnectionString, azureStorageContainerName));
            builder.Services.AddSingleton<ILoggerProvider, EAMLoggerProvider>();
        }
    }
}
