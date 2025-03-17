using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models.DomainModel
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }
        public required string ProductName { get; set;}
        public required int ProductPrice { get; set; }
        public required int ProductQuantity { get; set;}
        public required string Category {  get; set;}
        public required string SubCategory { get; set;}
        public bool IsArchived { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string? ProductDescription { get; set; }
        public required string ProductPicUrl { get; set; }


    

        // One-to-many relationship: A product can appear in many cart items
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public required Guid SellerId { get; set; }
        [ForeignKey("SellerId")]
        public User? Seller { get; set; }


        public ICollection<OrderProduct> OrderProducts { get; set; } = [];


    }
}
