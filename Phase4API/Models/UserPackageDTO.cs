namespace Phase4API.Models
{
    public class UserPackageDTO
    {
        public int PackageId { get; set; }
        public int UserId { get; set; }
        public int PackagePrice { get; set; }
        public int Count { get; set; }
        public DateOnly Date { get; set; }
        public string FlightName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FlightType { get; set; }
        public string AccName { get; set; }
        public string AccType { get; set; }
        public string AccAddress { get; set; }
        public string SSNames { get; set; }
        public bool Luxuary { get; set; }
    }
}
