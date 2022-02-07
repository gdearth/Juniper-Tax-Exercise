using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;
using Interface.ServiceAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BusinessUtility.Tests;

public class TaxBuTests
{
    [Fact]
    public async Task GetTaxRateByLocation_USZipPlusFourOnly_ReturnTaxRate()
    {
        var taxSaMock = new Mock<ITaxSa>();
        taxSaMock.Setup(x => x.GetTaxRateByLocation(It.IsAny<Location>(), It.IsAny<CancellationToken>())).ReturnsAsync(0.0975m);
        var factoryMock = new Mock<ITaxFactory>();
        factoryMock.Setup(x => x.GetTaxProvider(It.IsAny<Customer>()))
            .Returns(taxSaMock.Object);
        var logger = GetLogger<TaxBu>();

        var location = new Location
        {
            ZipCode = "90404-3370"
        };
        
        var businessUtility = new TaxBu(factoryMock.Object, logger);
        var taxRate = await businessUtility.GetTaxRateByLocation(location, CancellationToken.None);
            
        Assert.Equal(0.0975m, taxRate);
    }

    [Fact]
    public async Task GetTaxByOrder_USOrder_ReturnsTotalTax()
    {
        var taxSaMock = new Mock<ITaxSa>();
        taxSaMock.Setup(x => x.GetTaxByOrder(It.IsAny<Order>(), It.IsAny<CancellationToken>())).ReturnsAsync(1.35m);
        var factoryMock = new Mock<ITaxFactory>();
        factoryMock.Setup(x => x.GetTaxProvider(It.IsAny<Customer>()))
            .Returns(taxSaMock.Object);
        var logger = GetLogger<TaxBu>();

        var order = new Order
        {
            FromCountry = "US",
            FromZip = "92093",
            FromState = "CA",
            FromCity = "La Jolla",
            FromStreet = "9500 Gilman Drive",
            ToCountry = "US",
            ToZip = "90002",
            ToState = "CA",
            ToCity = "Los Angeles",
            ToStreet = "1335 E 103rd St",
            Amount = 15,
            Shipping = 1.5m,
            NexusAddresses = new List<NexusAddress>
            {
                new()
                {
                    Id = "Main Location",
                    Country = "US",
                    Zip = "92093",
                    State = "CA",
                    City = "La Jolla",
                    Street = "9500 Gilman Drive"
                }
            },
            LineItems = new List<LineItem>
            {
                new()
                {
                    Id = "1",
                    Quantity = 1,
                    ProductTaxCode = "20010",
                    UnitPrice = 15,
                    Discount = 0
                }
            }
        };

        var businessUtility = new TaxBu(factoryMock.Object, logger);
        var taxRate = await businessUtility.GetTaxByOrder(order, new CancellationToken());

        Assert.Equal(1.35m, taxRate);
    }
    
    private ILogger<T> GetLogger<T>()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        var logger = factory.CreateLogger<T>();

        return logger;
    }
}