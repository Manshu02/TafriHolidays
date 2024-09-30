using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Phase4API.Models
{
    

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int ProducerId { get; set; }

        // Navigation property to represent the many-to-one relationship

        [JsonIgnore]
        public Producer Producer { get; set; }
    }

}
