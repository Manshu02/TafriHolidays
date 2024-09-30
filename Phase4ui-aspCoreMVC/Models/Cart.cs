using System.ComponentModel.DataAnnotations;

namespace Phase4.Models
{
    public class Cart
    {
        [Key]
        public int cartId { get; set; }
        public int userId { get; set; }
        public int packageId { get; set; }
    }
}
