using server.Models;

namespace server.Bll.Interfaces
{
    public interface ILotteryService
    {
        Task<Ticket> DoLottery(int giftId);
        Task<List<Ticket>> GetWinnersReport();
        Task<decimal> GetTotalIncome();
    }
}
