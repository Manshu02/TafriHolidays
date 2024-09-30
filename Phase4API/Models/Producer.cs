using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Phase4API.Models
{
   

    public class Producer
    {
        [Key]
        public int ProducerId { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation property to represent the one-to-many relationship
        public ICollection<Product> Products { get; set; }
    }

}
