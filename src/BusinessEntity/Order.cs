using System.Collections.Generic;

namespace BusinessEntity;

public class Order : Customer
{
    public string FromCountry { get; set; }
    public string FromZip { get; set; }
    public string FromState { get; set; }
    public string FromCity { get; set; }
    public string FromStreet { get; set; }
    public string ToCountry { get; set; }
    public string ToZip { get; set; }
    public string ToState { get; set; }
    public string ToCity { get; set; }
    public string ToStreet { get; set; }
    public decimal Amount { get; set; }
    public decimal Shipping { get; set; }
    public string CustomerId { get; set; }
    public string ExemptionType { get; set; }
    public IList<NexusAddress> NexusAddresses { get; set; } = new List<NexusAddress>();
    public IList<LineItem> LineItems { get; set; } = new List<LineItem>();
}