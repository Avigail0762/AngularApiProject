using server.Models;
using System.Threading.Tasks;

namespace server.Dal.Interfaces
{
    public interface ICustomerDal
    {
        // ---------- USER ----------
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<User> AddUser(User user);
        Task UpdateUser(User user);

        // ---------- TICKETS ----------
        Task AddTicket(Ticket ticket);
        Task UpdateTicket(Ticket ticket);

    }
}
