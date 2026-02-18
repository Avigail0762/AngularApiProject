using server.Models;
using server.Models.DTO;

namespace server.Bll.Interfaces
{
    public interface IGiftService
    {
        Task<Gift> Add(GiftDTO gifted);
        Task<bool> Remove(int id);
        Task Update(int id, GiftDTO updateGift);
        Task<Gift?> GetByName(string firstname);
        Task<List<Gift>?> GetByDonorName(string firstName, string lastName);
        Task<List<Gift>?> GetByBuyersNumber(int number);
        Task<Gift?> GetById(int id);
        Task<List<Gift>> Get();
        Task<List<Gift>> GetByCategory(string category);
        Task<List<Gift>> GetByPrice(bool ascending = true);
    }
}
