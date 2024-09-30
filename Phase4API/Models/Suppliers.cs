using System.ComponentModel.DataAnnotations;

namespace Phase4API.Models
{
    public class Suppliers
    {
        [Key]
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public long SupplierContact { get; set; }
        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierPassword { get; set; }
        public string Type { get; set; } = "supplier";

      
        public bool RegisterationStatus { get; set; } = false;
    }
}
