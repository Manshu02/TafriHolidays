namespace Phase4API.Models
{
    public class PackageSpecificationViewModel
    {
        public int PackageId { get; set; }
        public int UserId { get; set; }
        public string FlightName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FlightType { get; set; }
        public string AccName { get; set; }
        public string AccType { get; set; }
        public string AccAddress { get; set; }
        public string SSNames { get; set; }
        public bool Luxuary { get; set; }
        public float PackagePrice { get; set; }
        public int AdminPrice { get; set; }
        public int TotalCount { get; set; }
        public int UsedCount { get; set; }
    }
}
