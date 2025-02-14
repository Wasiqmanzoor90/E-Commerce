using E_Commerce.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models.DomainModel
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public required int Orderquantity { get; set; }
        public Status Status { get; set; } // Add this property
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        // One-to-one relationship: One order belongs to one cart 
        // Because in the Cart model, we have CartItems which can represent
        // multiple products and can be associated with multiple orders
        public Guid CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart? cart { get; set; }

        //Many-to-One relationship:Many order belong to one user
        public Guid UserId { get; set; }
        // Navigation property to the User model
        [ForeignKey("UserId")]
        public User? user { get; set; }


    }
}
