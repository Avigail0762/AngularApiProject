using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Gift
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)] 
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
        public int DonorId { get; set; }

        [Required]
        public Donor Donor { get; set; }
        [Range(10, 100)]
        public int Price { get; set; }
        public int BuyersNumber { get; set; }

        [StringLength(100)] 
        public string Category { get; set; }
        public int? WinnerTicketId { get; set; }
        [DefaultValue(false)]
        public bool IsDrawn { get; set; } = false;

    }
}
