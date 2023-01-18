using System.ComponentModel.DataAnnotations;

namespace EShopCore3.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string? AdminName { get; set; }

        [DataType(DataType.Password)]
        public string? AdminPassword { get; set; }
    }
}
