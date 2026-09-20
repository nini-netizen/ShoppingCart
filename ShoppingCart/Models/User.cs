using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(256)]
        public string Email { get; set; } = "";

        [Required, StringLength(100)]
        public string Name { get; set; } = "";
    }
}
