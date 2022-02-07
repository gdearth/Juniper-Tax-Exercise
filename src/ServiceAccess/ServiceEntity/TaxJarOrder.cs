using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ServiceAccess.ServiceEntity;

internal class TaxJarOrder
{
    [JsonPropertyName("from_country")] public string FromCountry { get; set; }
    [JsonPropertyName("from_zip")] public string FromZip { get; set; }
    [JsonPropertyName("from_state")] public string FromState { get; set; }
    [JsonPropertyName("from_city")] public string FromCity { get; set; }
    [JsonPropertyName("from_street")] public string FromStreet { get; set; }
    [JsonPropertyName("to_country")] public string ToCountry { get; set; }
    [JsonPropertyName("to_zip")] public string ToZip { get; set; }
    [JsonPropertyName("to_state")] public string ToState { get; set; }
    [JsonPropertyName("to_city")] public string ToCity { get; set; }
    [JsonPropertyName("to_street")] public string ToStreet { get; set; }
    [JsonPropertyName("amount")] public decimal Amount { get; set; }
    [JsonPropertyName("shipping")] public decimal Shipping { get; set; }
    [JsonPropertyName("customer_id")] public string CustomerId { get; set; }
    [JsonPropertyName("exemption_type")] public string ExemptionType { get; set; }
    [JsonPropertyName("nexus_addresses")] public IList<TaxJarNexusAddress> NexusAddresses { get; set; } = new List<TaxJarNexusAddress>();
    [JsonPropertyName("line_items")] public IList<TaxJarLineItem> LineItems { get; set; } = new List<TaxJarLineItem>();
}