namespace ShoppingCart.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; } = "Active";

        public User? User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
