using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;
using Interface.BusinessUtility;
using Interface.ServiceAccess;
using Microsoft.Extensions.Logging;

namespace BusinessUtility
{
    public class TaxBu : ITaxBu
    {
        private readonly ITaxFactory _taxFactory;
        private readonly ILogger _logger;

        public TaxBu(ITaxFactory taxFactory, ILogger<TaxBu> logger)
        {
            _taxFactory = taxFactory;
            _logger = logger;
        }
        
        public async Task<decimal> GetTaxRateByLocation(Location location, CancellationToken token)
        {
            using (_logger.BeginScope("Starting {0}", nameof(GetTaxRateByLocation)))
            {
                var taxProvider = _taxFactory.GetTaxProvider(location);

                _logger.LogDebug($"Using {taxProvider.GetType().Name}");

                var taxRate = await taxProvider.GetTaxRateByLocation(location, token);

                _logger.LogTrace("Tax Rate: {0}", taxRate);
                
                return taxRate;
            }
        }

        public async Task<decimal> GetTaxByOrder(Order order, CancellationToken token)
        {
            using (_logger.BeginScope("Starting {0}", nameof(GetTaxByOrder)))
            {
                var taxProvider = _taxFactory.GetTaxProvider(order);
                
                _logger.LogDebug($"Using {taxProvider.GetType().Name}");
                
                var tax = await taxProvider.GetTaxByOrder(order, token);
                
                _logger.LogTrace("Tax Amount: {0}", tax);
                
                return tax;
            }
        }
    }
}
