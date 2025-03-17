using E_Commerce.Models.DomainModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models.JunctionModel
{
    public class OrderProduct
    {
        public Guid OrderProductId { get; set; } = Guid.NewGuid();


        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? order { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? product { get; set; }

        public int Quantity { get; set; }

    }
}
