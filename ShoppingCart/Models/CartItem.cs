using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        public int CartId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }
}
