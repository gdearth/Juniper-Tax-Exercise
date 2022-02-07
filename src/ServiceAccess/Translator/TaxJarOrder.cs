using System.Collections.Generic;
using System.Linq;
using BusinessEntity;
using ServiceAccess.ServiceEntity;

namespace ServiceAccess.Translator;

internal static class TaxJarOrderTranslator
{
    internal static TaxJarOrder Translate(this Order order)
    {
        return new TaxJarOrder
        {
            FromCountry = order.FromCountry,
            FromZip = order.FromZip,
            FromState = order.FromState,
            FromCity = order.FromCity,
            FromStreet = order.FromStreet,
            ToCountry = order.ToCountry,
            ToZip = order.ToZip,
            ToState = order.ToState,
            ToCity = order.ToCity,
            ToStreet = order.ToStreet,
            Amount = order.Amount,
            Shipping = order.Shipping,
            CustomerId = order.CustomerId,
            ExemptionType = order.ExemptionType,
            NexusAddresses = order.NexusAddresses.Translate().ToList(),
            LineItems = order.LineItems.Translate().ToList()
        };
    }

    internal static IEnumerable<TaxJarOrder> Translate(this IEnumerable<Order> orders) =>
        orders.Select(x => x.Translate());
}