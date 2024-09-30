using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phase4.Models
{
    public class AdminUserWishList
    {
        [Key]
        public int AdminUserWishListID { get; set; }

        
        public int PackageID { get; set; }
        public int UserID { get; set; }
        
    }
}
