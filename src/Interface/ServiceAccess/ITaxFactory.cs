using BusinessEntity;

namespace Interface.ServiceAccess;

public interface ITaxFactory
{
    ITaxSa GetTaxProvider(Customer customer);
}