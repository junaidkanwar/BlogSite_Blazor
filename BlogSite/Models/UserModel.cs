using System.ComponentModel.DataAnnotations;

namespace BlogSite.Models
{
    public class UserModel
    {
        public int UserId { get; set; } // New UserId field

        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
