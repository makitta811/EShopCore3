using System.ComponentModel.DataAnnotations;

namespace EShopCore3.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }

        [DataType(DataType.Password)]
        public string? UserPassword { get; set; }
        public string? UserPhoto { get; set; }
    }
}
