using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

[assembly: FunctionsStartup(typeof(ModelConversionService.Startup))]

namespace ModelConversionService
{
    public class Startup : FunctionsStartup {

       public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, EAMLoggerProvider>();
        }
    }
}