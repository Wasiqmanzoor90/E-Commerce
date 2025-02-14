using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModel
{
    public class Login
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
