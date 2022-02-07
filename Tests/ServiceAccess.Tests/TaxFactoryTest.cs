using System.Net.Http;
using BusinessEntity;
using Interface.ServiceAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ServiceAccess.Tests;

public class TaxFactoryTest
{
    [Fact]
    public void GetTaxProvider_TaxJarCustomer_ReturnsTaxJar()
    {
        var customer = new Customer { TaxHandler = "TaxJar" };

        var serviceProvider = new ServiceCollection()
            .AddScoped<TaxJarTaxSa>()
            .AddScoped<ITaxSa, TaxJarTaxSa>(s=>s.GetService<TaxJarTaxSa>())
            .AddScoped<ITaxFactory, TaxFactory>()
            .AddSingleton<HttpClient>()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ITaxFactory>();
        
        Assert.NotNull(factory);
        
        var taxProvider = factory.GetTaxProvider(customer);
        
        Assert.Equal(typeof(TaxJarTaxSa), taxProvider.GetType());
    }
    
    [Fact]
    public void GetTaxProvider_EmptyCustomer_ReturnsTaxJar()
    {
        var customer = new Customer { TaxHandler = string.Empty };

        var serviceProvider = new ServiceCollection()
            .AddScoped<TaxJarTaxSa>()
            .AddScoped<ITaxSa, TaxJarTaxSa>(s => s.GetService<TaxJarTaxSa>())
            .AddScoped<ITaxFactory, TaxFactory>()
            .AddSingleton<HttpClient>()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ITaxFactory>();
        
        Assert.NotNull(factory);
        
        var taxProvider = factory.GetTaxProvider(customer);
        
        Assert.Equal(typeof(TaxJarTaxSa), taxProvider.GetType());
    }
}