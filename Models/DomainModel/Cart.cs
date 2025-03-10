using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models.DomainModel
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? user { get; set; }

        public decimal Totalprice { get; set; }
        public int CartQuantity { get; set; }

        // One-to-many relation with CartItems  
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        // One-to-many relation with Orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }

    public class CartItem
    {
        [Key]
        public Guid CartItemId { get; set; }
        // Many-to-One relationship: Many CartItems belong to one Cart
        public Guid CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; } // Allows the user to add multiple of the same product
        public decimal Price { get; set; } // Stores the price at the time of adding to cart
    }
}
