using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;
using System.Net.Http.Json;
using Interface.ServiceAccess;
using ServiceAccess.ServiceEntity;
using ServiceAccess.Translator;

namespace ServiceAccess
{
    public class TaxJarTaxSa : ITaxSa
    {
        private readonly HttpClient _client;

        public TaxJarTaxSa(HttpClient client)
        {
            _client = client;
        }
        
        public async Task<decimal> GetTaxRateByLocation(Location location, CancellationToken token)
        {
            var queryString = location.ToQueryString();
            var path = string.IsNullOrEmpty(location.ZipCode) ? "v2/rates" : $"v2/rates/{location.ZipCode}";
            var uriBuilder = new UriBuilder(_client.BaseAddress)
            {
                Scheme = "https",
                Path = path
            };
            
            if (!string.IsNullOrWhiteSpace(queryString))
                uriBuilder.Query = queryString;

            var response = await _client.GetFromJsonAsync<TaxRate>(uriBuilder.ToString(), token);

            var taxRate = response?.Rate?.CombinedRate ?? response?.Rate?.StandardRate;

            if (taxRate == null)
                throw new InvalidDataException("Api returned invalid tax rate");

            return (decimal)taxRate;
        }

        public async Task<decimal> GetTaxByOrder(Order order, CancellationToken token)
        {
            var taxJarOrder = order.Translate();
            var response = await _client.PostAsJsonAsync("v2/taxes", taxJarOrder, token);
            
            response.EnsureSuccessStatusCode();

            var salesTax = await response.Content.ReadFromJsonAsync<TaxJarSalesTax>(cancellationToken: token);
            var tax = salesTax?.Tax?.AmountToCollect;
            if (tax==null)
                throw new InvalidDataException("Api returned invalid tax to collect");
            return (decimal)tax;
        }
    }
}
