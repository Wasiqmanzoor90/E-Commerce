using E_Commerce.Models.JunctionModel;
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
        public int CartQuantity {  get; set; }
        public decimal Totalprice { get; set; }

        // One-to-many relation with CartItems  
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        // One-to-many relation with Orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }

   
}
