using Microsoft.EntityFrameworkCore;
using server.Dal.Interfaces;
using server.Models;

namespace server.Dal
{
    public class PurchasesDal : IPurchasesDal
    {
        private readonly SaleContext saleContext;

        public PurchasesDal(SaleContext context)
        {
            saleContext = context;
        }

        // כל הכרטיסים של מתנה מסוימת
        public async Task<List<Ticket>> GetTicketsByGiftId(int giftId)
        {
            return await saleContext.Tickets.AsNoTracking()
                           .Where(t => t.GiftId == giftId)
                           .ToListAsync();
        }

        // מיון לפי המתנה היקרה ביותר 
        public async Task<List<Gift>> GetGiftsSortedByPrice()
        {
            return await saleContext.Gifts.AsNoTracking()
                           .OrderByDescending(g => g.Price)
                           .ToListAsync();
        }

        // מיון לפי המתנה הנרכשת ביותר 
        public async Task<List<Gift>> GetGiftsSortedByPurchases()
        {
            return await saleContext.Gifts.AsNoTracking()
                           .OrderByDescending(g => g.BuyersNumber)
                           .ToListAsync();
        }

        // צפייה בפרטי הרוכשים עבור מתנה מסוימת 
        public async Task<List<User>> GetBuyersByGiftId(int giftId)
        {
            // שליפת כל הכרטיסים של המתנה
            var tickets = await saleContext.Tickets.AsNoTracking()
                                  .Where(t => t.GiftId == giftId)
                                  .ToListAsync();

            // יצירת רשימת יוזרים בלי כפילויות
            //לזכור לשנות אחרי שמעדכנים את המודל של טיקט לפי כמות
            List<User> buyers = new List<User>();

            foreach (var t in tickets)
            {
                var user = await saleContext.Users.FirstOrDefaultAsync(u => u.Id == t.UserId);
                if (user != null && !buyers.Any(b => b.Id == user.Id))
                {
                    buyers.Add(user);
                }
            }

            return buyers;
        }
    }
}
