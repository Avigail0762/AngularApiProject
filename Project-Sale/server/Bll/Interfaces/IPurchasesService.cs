using server.Models;

namespace server.Bll.Interfaces
{
    public interface IPurchasesService
    {
        Task<List<Ticket>> GetTicketsByGiftId(int giftId);
        Task<List<Gift>> GetGiftsSortedByPrice();
        Task<List<Gift>> GetGiftsSortedByPurchases();
        Task<List<User>> GetBuyersByGiftId(int giftId);
    }
}
