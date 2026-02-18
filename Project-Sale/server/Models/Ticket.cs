using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int GiftId { get; set; }
        public Gift Gift { get; set; }
        public int TicketNumberForGift { get; set; } 
        public int Quantity { get; set; } = 1;

        [Required]
        [DataType(DataType.DateTime)] 
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    }
}
