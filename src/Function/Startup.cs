using System;
using System.Net.Http.Headers;
using BusinessUtility;
using Interface.BusinessUtility;
using Interface.ServiceAccess;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceAccess;

[assembly: FunctionsStartup(typeof(Function.Startup))]
namespace Function;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Debug));
        
        builder.Services.AddHttpClient<TaxJarTaxSa>("TaxJar", client =>
        {
            client.BaseAddress = new Uri("https://api.taxjar.com/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");
        });
        
        builder.Services.AddScoped<ITaxSa, TaxJarTaxSa>(s => s.GetService<TaxJarTaxSa>());
        builder.Services.AddScoped<ITaxFactory, TaxFactory>();
        
        builder.Services.AddScoped<ITaxBu, TaxBu>();
    }
}