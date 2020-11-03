using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(UUIDService.Startup))]

namespace UUIDService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(new AzureServiceTokenProvider());
            builder.Services.AddDbContext<UUIDDataContext>();
            builder.Services.AddSingleton<ILoggerProvider, EAMLoggerProvider>();
        }
    }
}