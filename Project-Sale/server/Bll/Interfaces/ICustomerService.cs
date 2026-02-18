using server.Models;
using server.Models.DTO;

namespace server.Bll.Interfaces
{
    public interface ICustomerService
    {
        // ---------- AUTH ----------
        Task<User> Register(UserDTO dto);
        // ---------- GIFTS ----------
        Task<List<Gift>> GetGifts(string? category, bool? sortPriceAsc);

        // ---------- CART ----------
        Task AddToCart(int userId, int giftId);
        Task RemoveFromCart(int userId, int giftId);
        Task<List<Gift>> GetCart(int userId);
        // ---------- PURCHASE ----------
        Task Purchase(int userId);
    }
}
