using System.Threading;
using System.Threading.Tasks;
using BusinessEntity;

namespace Interface.ServiceAccess;

public interface ITaxSa
{
    Task<decimal> GetTaxRateByLocation(Location location, CancellationToken token);
    Task<decimal> GetTaxByOrder(Order order, CancellationToken token);
}