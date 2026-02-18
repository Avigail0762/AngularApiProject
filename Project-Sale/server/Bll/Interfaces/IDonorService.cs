using server.Models;
using server.Models.DTO;

namespace server.Bll.Interfaces
{
    public interface IDonorService
    {
        Task<Donor> Add(DonorDTO donored);
        Task<bool> Remove(int id);
        Task Update(int id, DonorDTO updateDonor);
        Task<Donor?> GetByName(string firstName, string lastName);
        Task<Donor?> GetByEmail(string email);
        Task<Donor?> GetById(int id);
        Task<Donor?> GetByGift(Gift gift);
        Task<List<Donor>> Get();
    }
}
