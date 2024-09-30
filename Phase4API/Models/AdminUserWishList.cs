using System.ComponentModel.DataAnnotations;

namespace Phase4API.Models
{
    public class AdminUserWishList
    {
        [Key]
        public int AdminUserWishListID { get; set; }


        public int PackageID { get; set; }
        public int UserID { get; set; }
        
    }
}
