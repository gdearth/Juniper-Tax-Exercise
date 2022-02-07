namespace BusinessEntity
{
    public class Location : Customer
    {
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
    }
}
