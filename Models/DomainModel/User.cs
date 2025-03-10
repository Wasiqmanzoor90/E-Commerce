using E_Commerce.Types;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.DomainModel
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Role Role { get; set; } = Role.Buyer;
        public string? ProfilePictureUrl { get; set; }




        public Cart? Cart { get; set; }  // Navigation property (One-to-One relationship)

        // Navigation properties (one to many) so the list need
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<Product> Products { get; set; } = [];
        public ICollection<Adress> Adresses { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}






