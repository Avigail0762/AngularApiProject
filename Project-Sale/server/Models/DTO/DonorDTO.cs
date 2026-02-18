using System.ComponentModel.DataAnnotations;

namespace server.Models.DTO
{
    public class DonorDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }
    }
}
