using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Phase4.Models
{
    public class Admins
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminID { get; set; }

        [Required]
        [StringLength(255)]
        public string AdminName { get; set; }

        [Required]
        public long AdminContact { get; set; }

        [Required]
        [StringLength(255)]
        public string AdminEmail { get; set; }

        [Required]
        [StringLength(255)]
        public string AdminPassword { get; set; }

        [StringLength(255)]
        public string Type { get; set; } = "admin";
    }
}
