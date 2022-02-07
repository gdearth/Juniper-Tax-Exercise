using System;
using BusinessEntity;
using Interface.ServiceAccess;

namespace ServiceAccess;

public class TaxFactory : ITaxFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TaxFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITaxSa GetTaxProvider(Customer customer)
    {
        switch (customer.TaxHandler)
        {
            case "TaxJar":
            default:
                return (ITaxSa)_serviceProvider.GetService(typeof(TaxJarTaxSa));
        } 
    }
}