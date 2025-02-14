using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models.DomainModel
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; }
        public string? CreateReview { get; set; }

        public DateTime CreateDate { get; set; }

        public required Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? user { get; set; }


    }
}
