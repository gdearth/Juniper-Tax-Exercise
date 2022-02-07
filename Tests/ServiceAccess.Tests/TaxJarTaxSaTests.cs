using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;
using Moq;
using Moq.Protected;
using Xunit;

namespace ServiceAccess.Tests;

public class TaxJarTaxSaTests
{
    [Fact]
    public async Task GetTaxRateByLocation_USZipPlusFourCodeOnly_ReturnsTaxRate()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{""rate"": { ""zip"": ""90404"", ""state"": ""CA"", ""state_rate"": ""0.0625"", ""county"": ""LOS ANGELES"", ""county_rate"": ""0.01"", ""city"": ""SANTA MONICA"", ""city_rate"": ""0.0"", ""combined_district_rate"": ""0.025"", ""combined_rate"": ""0.0975"", ""freight_taxable"": false}}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var location = new Location
        {
            ZipCode = "90404-3370"
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxRateByLocation(location, new CancellationToken());

        Assert.Equal(0.0975m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Get && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/rates/90404-3370" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxRateByLocation_USNoZip_ThrowsException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent(
                @"{""status"": 400,""error"": ""Bad Request"",""detail"": ""No zip, required when country is US""}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var location = new Location
        {
            City ="Santa Monica",
            State="CA",
            CountryCode = "US"
        };

        var taxJar = new TaxJarTaxSa(client);
        await Assert.ThrowsAsync<HttpRequestException>(async () => await taxJar.GetTaxRateByLocation(location, new CancellationToken()));
        
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Get && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/rates" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxRateByLocation_USZipCodeCityStateCountry_ReturnsTaxRate()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{""rate"": { ""zip"": ""90404"", ""state"": ""CA"", ""state_rate"": ""0.0625"", ""county"": ""LOS ANGELES"", ""county_rate"": ""0.01"", ""city"": ""SANTA MONICA"", ""city_rate"": ""0.0"", ""combined_district_rate"": ""0.025"", ""combined_rate"": ""0.0975"", ""freight_taxable"": false}}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var location = new Location
        {
            ZipCode = "90404",
            City ="Santa Monica",
            State="CA",
            CountryCode = "US"
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxRateByLocation(location, new CancellationToken());

        Assert.Equal(0.0975m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Get && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/rates/90404" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxRateByLocation_USZipCodePlusFourStreetCityStateCountry_ReturnsTaxRate()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{ ""rate"": { ""zip"": ""05495-2086"", ""country"": ""US"", ""country_rate"": ""0.0"", ""state"": ""VT"", ""state_rate"": ""0.06"", ""county"": ""CHITTENDEN"", ""county_rate"": ""0.0"", ""city"": ""WILLISTON"", ""city_rate"": ""0.0"", ""combined_district_rate"": ""0.01"", ""combined_rate"": ""0.07"", ""freight_taxable"": true }}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var location = new Location
        {
            ZipCode = "05495-2086",
            Street = "312 Hurricane Lane",
            City ="Williston",
            State="VT",
            CountryCode = "US"
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxRateByLocation(location, new CancellationToken());

        Assert.Equal(0.07m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Get && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/rates/05495-2086" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxRateByLocation_CAZipCityState_ReturnsTaxRate()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{ ""rate"": { ""zip"": ""V5K0A1"", ""city"": ""Vancouver"", ""state"": ""BC"", ""country"": ""CA"", ""combined_rate"": ""0.12"", ""freight_taxable"": true }}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var location = new Location
        {
            ZipCode = "V5K0A1",
            City="Vancouver",
            State = "BC",
            CountryCode = "CA"
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxRateByLocation(location, new CancellationToken());

        Assert.Equal(0.12m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Get && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/rates/V5K0A1" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxRateByLocation_AUZipCityState_ReturnsTaxRate()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{ ""rate"": { ""zip"": ""2060"", ""country"": ""AU"", ""country_rate"": ""0.1"", ""combined_rate"": ""0.1"", ""freight_taxable"": true }}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var location = new Location
        {
            ZipCode = "2060",
            City="Sydney",
            CountryCode = "AU"
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxRateByLocation(location, new CancellationToken());

        Assert.Equal(0.1m, taxRate);
        
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Get && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/rates/2060" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxRateByLocation_EUZipCityState_ReturnsTaxRate()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{""rate"": {  ""country"": ""FI"",  ""name"": ""Finland"",  ""standard_rate"": ""0.24"",  ""reduced_rate"": ""0.0"",  ""super_reduced_rate"": ""0.0"",  ""parking_rate"": ""0.0"",  ""distance_sale_threshold"": ""0.0"",  ""freight_taxable"": true}}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var location = new Location
        {
            ZipCode = "00150",
            City="Helsinki",
            CountryCode = "FI"
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxRateByLocation(location, new CancellationToken());

        Assert.Equal(0.24m, taxRate);
        
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Get && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/rates/00150" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxByOrder_USOrder_ReturnsTotalTax()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{""tax"": {""order_total_amount"": 16.5, ""shipping"": 1.5, ""taxable_amount"": 15, ""amount_to_collect"": 1.35, ""rate"": 0.09, ""has_nexus"": true, ""freight_taxable"": false, ""tax_source"": ""destination"", ""jurisdictions"": { ""country"": ""US"", ""state"": ""CA"", ""county"": ""LOS ANGELES"",  ""city"": ""LOS ANGELES"" }, ""breakdown"": { ""taxable_amount"": 15, ""tax_collectable"": 1.35, ""combined_tax_rate"": 0.09, ""state_taxable_amount"": 15, ""state_tax_rate"": 0.0625, ""state_tax_collectable"": 0.94, ""county_taxable_amount"": 15, ""county_tax_rate"": 0.0025, ""county_tax_collectable"": 0.04, ""city_taxable_amount"": 0, ""city_tax_rate"": 0, ""city_tax_collectable"": 0, ""special_district_taxable_amount"": 15, ""special_tax_rate"": 0.025, ""special_district_tax_collectable"": 0.38, ""line_items"": [{ ""id"": ""1"", ""taxable_amount"": 15, ""tax_collectable"": 1.35, ""combined_tax_rate"": 0.09, ""state_taxable_amount"": 15, ""state_sales_tax_rate"": 0.0625,""state_amount"": 0.94,""county_taxable_amount"": 15,""county_tax_rate"": 0.0025,""county_amount"": 0.04,""city_taxable_amount"": 0,""city_tax_rate"": 0,""city_amount"": 0,""special_district_taxable_amount"": 15,""special_tax_rate"": 0.025,""special_district_amount"": 0.38 }]}}}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

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

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxByOrder(order, new CancellationToken());

        Assert.Equal(1.35m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Post && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/taxes" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxByOrder_CAOrder_ReturnsTotalTax()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{""tax"": {""order_total_amount"": 16.5,""shipping"": 1.5,""taxable_amount"": 16.5,""amount_to_collect"": 2.15,""rate"": 0.13,""has_nexus"": true,""freight_taxable"": true,""tax_source"": ""destination"",""jurisdictions"": {""country"": ""CA"",""state"": ""ON"",""city"": ""TORONTO""},""breakdown"": {""combined_tax_rate"": 0.13,""gst"": 0.83,""gst_tax_rate"": 0.05,""gst_taxable_amount"": 16.5,""line_items"": [{""combined_tax_rate"": 0.13,""gst"": 0.75,""gst_tax_rate"": 0.05,""gst_taxable_amount"": 15,""id"": ""1"",""pst"": 1.2,""pst_tax_rate"": 0.08,""pst_taxable_amount"": 15,""qst"": 0,""qst_tax_rate"": 0,""qst_taxable_amount"": 0,""tax_collectable"": 1.95,""taxable_amount"": 15}],""pst"": 1.32,""pst_tax_rate"": 0.08,""pst_taxable_amount"": 16.5,""qst"": 0,""qst_tax_rate"": 0,""qst_taxable_amount"": 0,""shipping"": {""combined_tax_rate"": 0.13,""gst"": 0.08,""gst_tax_rate"": 0.05,""gst_taxable_amount"": 1.5,""pst"": 0.12,""pst_tax_rate"": 0.08,""pst_taxable_amount"": 1.5,""qst"": 0,""qst_tax_rate"": 0,""qst_taxable_amount"": 0,""tax_collectable"": 0.2,""taxable_amount"": 1.5},""tax_collectable"": 2.15,""taxable_amount"": 16.5}}}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var order = new Order
        {
            FromCountry = "CA",
            FromZip = "V6G 3E2",
            FromState = "BC",
            FromCity = "Vancouver",
            FromStreet = "845 Avison Way",
            ToCountry = "CA",
            ToZip = "M5V 2T6",
            ToState = "ON",
            ToCity = "Toronto",
            ToStreet = "301 Front St W",
            Amount = 15,
            Shipping = 1.5m,
            NexusAddresses = new List<NexusAddress>
            {
                new()
                {
                    Id = "Main Location",
                    Country = "CA",
                    Zip = "V6G 3E2",
                    State = "BC",
                    City = "Vancouver",
                    Street = "845 Avison Way"
                }
            },
            LineItems = new List<LineItem>
            {
                new()
                {
                    Id = "1",
                    Quantity = 1,
                    UnitPrice = 15,
                    Discount = 0
                }
            }
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxByOrder(order, new CancellationToken());

        Assert.Equal(2.15m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Post && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/taxes" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxByOrder_AUOrder_ReturnsTotalTax()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{""tax"": {""order_total_amount"": 16.5,""shipping"": 1.5,""taxable_amount"": 16.5,""amount_to_collect"": 2.15,""rate"": 0.13,""has_nexus"": true,""freight_taxable"": true,""tax_source"": ""destination"",""jurisdictions"": {""country"": ""CA"",""state"": ""ON"",""city"": ""TORONTO""},""breakdown"": {""combined_tax_rate"": 0.13,""gst"": 0.83,""gst_tax_rate"": 0.05,""gst_taxable_amount"": 16.5,""line_items"": [{""combined_tax_rate"": 0.13,""gst"": 0.75,""gst_tax_rate"": 0.05,""gst_taxable_amount"": 15,""id"": ""1"",""pst"": 1.2,""pst_tax_rate"": 0.08,""pst_taxable_amount"": 15,""qst"": 0,""qst_tax_rate"": 0,""qst_taxable_amount"": 0,""tax_collectable"": 1.95,""taxable_amount"": 15}],""pst"": 1.32,""pst_tax_rate"": 0.08,""pst_taxable_amount"": 16.5,""qst"": 0,""qst_tax_rate"": 0,""qst_taxable_amount"": 0,""shipping"": {""combined_tax_rate"": 0.13,""gst"": 0.08,""gst_tax_rate"": 0.05,""gst_taxable_amount"": 1.5,""pst"": 0.12,""pst_tax_rate"": 0.08,""pst_taxable_amount"": 1.5,""qst"": 0,""qst_tax_rate"": 0,""qst_taxable_amount"": 0,""tax_collectable"": 0.2,""taxable_amount"": 1.5},""tax_collectable"": 2.15,""taxable_amount"": 16.5}}}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var order = new Order
        {
            FromCountry = "AU",
            FromZip = "NSW 2000",
            FromCity = "Sydney",
            FromStreet = "483 George St",
            ToCountry = "AU",
            ToZip = "VIC 3002",
            ToCity = "Richmond",
            ToStreet = "Brunton Ave",
            Amount = 15,
            Shipping = 1.5m,
            NexusAddresses = new List<NexusAddress>
            {
                new()
                {
                    Id = "Main Location",
                    Country = "AU",
                    Zip = "NSW 2000",
                    City = "Sydney",
                    Street = "483 George St"
                }
            },
            LineItems = new List<LineItem>
            {
                new()
                {
                    Id = "1",
                    Quantity = 1,
                    UnitPrice = 15,
                    Discount = 0
                }
            }
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxByOrder(order, new CancellationToken());

        Assert.Equal(2.15m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Post && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/taxes" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetTaxByOrder_EUOrder_ReturnsTotalTax()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                @"{""tax"": {""order_total_amount"": 16.5,""shipping"": 1.5,""taxable_amount"": 16.5,""amount_to_collect"": 3.3,""rate"": 0.2,""has_nexus"": true,""freight_taxable"": true,""tax_source"": ""destination"",""jurisdictions"": {""country"": ""FR"",""city"": ""MARSEILLE""},""breakdown"": {""combined_tax_rate"": 0.2,""country_tax_collectable"": 3.3,""country_tax_rate"": 0.2,""country_taxable_amount"": 16.5,""line_items"": [{""combined_tax_rate"": 0.2,""country_tax_collectable"": 3,""country_tax_rate"": 0.2,""country_taxable_amount"": 15,""id"": ""1"",""tax_collectable"": 3,""taxable_amount"": 15}],""shipping"": {""combined_tax_rate"": 0.2,""country_tax_collectable"": 0.3,""country_tax_rate"": 0.2,""country_taxable_amount"": 1.5,""tax_collectable"": 0.3,""taxable_amount"": 1.5},""tax_collectable"": 3.3,""taxable_amount"": 16.5}}}")
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        var client = new HttpClient(handlerMock.Object);
        client.BaseAddress = new Uri("https://api.taxjar.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc");

        var order = new Order
        {
            FromCountry = "FR",
            FromZip = "75008",
            FromCity = "Paris",
            FromStreet = "55 Rue du Faubourg Saint-Honoré",
            ToCountry = "FR",
            ToZip = "13281",
            ToCity = "Marseille",
            ToStreet = "Rue Fort du Sanctuaire",
            Amount = 15,
            Shipping = 1.5m,
            NexusAddresses = new List<NexusAddress>
            {
                new()
                {
                    Id = "Main Location",
                    Country = "FR",
                    Zip = "75008",
                    City = "Paris",
                    Street = "55 Rue du Faubourg Saint-Honoré"
                }
            },
            LineItems = new List<LineItem>
            {
                new()
                {
                    Id = "1",
                    Quantity = 1,
                    UnitPrice = 15,
                    Discount = 0
                }
            }
        };

        var taxJar = new TaxJarTaxSa(client);
        var taxRate = await taxJar.GetTaxByOrder(order, new CancellationToken());

        Assert.Equal(3.3m, taxRate);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri != null && req.Method == HttpMethod.Post && req.RequestUri.Host == "api.taxjar.com" &&
                req.RequestUri.AbsolutePath == "/v2/taxes" &&
                Equals(req.Headers.Authorization,
                    new AuthenticationHeaderValue("Bearer", "5da2f821eee4035db4771edab942a4cc"))),
            ItExpr.IsAny<CancellationToken>());
    }
}