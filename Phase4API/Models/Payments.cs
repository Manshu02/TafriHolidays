using System.ComponentModel.DataAnnotations;

namespace Phase4API.Models
{
    public class Payments
    {
        [Key]
        public int paymentId { get; set; }
        public int packageId { get; set; }
        public int userId { get; set; }
        public int packagePrice { get; set; }
        public int AdminPrice { get; set; }
        public int count { get; set; }
        public DateOnly date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
