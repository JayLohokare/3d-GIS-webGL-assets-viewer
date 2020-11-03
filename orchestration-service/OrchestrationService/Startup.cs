using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchestrationService.Data;

[assembly: FunctionsStartup(typeof(OrchestrationService.Startup))]

namespace OrchestrationService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {            
            builder.Services.AddSingleton(new AzureServiceTokenProvider());
            builder.Services.AddDbContext<OrchestrationDataContext>();
            builder.Services.AddSingleton<ILoggerProvider, EAMLoggerProvider>();
        }
    }
}