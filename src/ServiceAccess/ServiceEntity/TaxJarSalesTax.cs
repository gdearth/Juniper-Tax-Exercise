using System.Text.Json.Serialization;

namespace ServiceAccess.ServiceEntity;

internal class TaxJarSalesTax
{
    [JsonPropertyName("tax")] public TaxJarTax Tax { get; set; } = new TaxJarTax();
}