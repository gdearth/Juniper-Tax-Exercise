using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BusinessEntity;
using ServiceAccess.ServiceEntity;

namespace ServiceAccess.Translator;

internal static class TaxJarNexusAddressTranslator
{
    internal static TaxJarNexusAddress Translate(this NexusAddress nexusAddress)
    {
        return new TaxJarNexusAddress
        {
            Id = nexusAddress.Id,
            Country = nexusAddress.Country,
            Zip = nexusAddress.Zip,
            State = nexusAddress.State,
            City = nexusAddress.City,
            Street = nexusAddress.Street
        };
    }

    internal static IEnumerable<TaxJarNexusAddress> Translate(this IEnumerable<NexusAddress> nexusAddresses) =>
        nexusAddresses.Select(x => x.Translate());
}