using server.Models;

namespace server.Dal.Interfaces
{
    public interface IPurchasesDal
    {
        Task<List<Ticket>> GetTicketsByGiftId(int giftId);
        Task<List<Gift>> GetGiftsSortedByPrice();
        Task<List<Gift>> GetGiftsSortedByPurchases();
        Task<List<User>> GetBuyersByGiftId(int giftId);
    }
}
