using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using UDHService.Data;

[assembly: FunctionsStartup(typeof(UDHService.Startup))]

namespace UDHService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(new AzureServiceTokenProvider());
            builder.Services.AddDbContext<UDHDataContext>();
            builder.Services.AddSingleton<ILoggerProvider, EAMLoggerProvider>();
        }
    }
}