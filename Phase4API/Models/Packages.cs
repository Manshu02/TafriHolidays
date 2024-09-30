using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phase4API.Models
{
    public class Packages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackageID { get; set; }

        [StringLength(255)]
        public string FlightName { get; set; }

        [StringLength(255)]
        public string From { get; set; }

        [StringLength(255)]
        public string To { get; set; }

        [StringLength(255)]
        public string FlightType { get; set; }

        [StringLength(255)]
        public string AccName { get; set; }

        [StringLength(255)]
        public string AccType { get; set; }

        [StringLength(255)]
        public string AccAddress { get; set; }

        [StringLength(255)]
        public string SSNames { get; set; }

        public bool Luxuary { get; set; }

        [Required]
        public int TotalCount { get; set; }

        public int UsedCount { get; set; } = 0;  // Added this property

        [Required]
        public DateTime Dated { get; set; } // Added this property

        [Required]
        public float PackagePrice { get; set; }  // Changed float to int

        public bool IsApproved { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [ForeignKey("Supplier")]
        public int SupplierID { get; set; }
        public int AdminPrice { get; set; } = 0;

    }
}
