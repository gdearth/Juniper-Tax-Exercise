using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;

namespace Interface.BusinessUtility;

public interface ITaxBu
{
    Task<decimal> GetTaxRateByLocation(Location location, CancellationToken token);
    Task<decimal> GetTaxByOrder(Order order, CancellationToken token);
}