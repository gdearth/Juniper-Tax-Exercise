using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BusinessEntity;
using ServiceAccess.ServiceEntity;

namespace ServiceAccess.Translator;

public static class TaxJarLineItemsTranslator
{
    internal static TaxJarLineItem Translate(this LineItem lineItem)
    {
        return new TaxJarLineItem
        {
            Id = lineItem.Id,
            Quantity = lineItem.Quantity,
            ProductTaxCode = lineItem.ProductTaxCode,
            UnitPrice = lineItem.UnitPrice,
            Discount = lineItem.Discount
        };
    }

    internal static IEnumerable<TaxJarLineItem> Translate(this IEnumerable<LineItem> lineItems) =>
        lineItems.Select(x => x.Translate());
}