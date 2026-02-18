using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100)] 
        public string Email { get; set; }
        [Required]
        public string? PasswordHash { get; set; }
        public List<int>? ShoppingCart { get; set; } = new List<int>();  
        public string Role { get; set; } = "user";
    }
}