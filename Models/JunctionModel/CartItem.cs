using E_Commerce.Models.DomainModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.JunctionModel
{
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
