using server.Bll.Interfaces;
using server.Dal.Interfaces;
using server.Models;

namespace server.Bll
{
    public class PurchasesService : IPurchasesService
    {
        private readonly IPurchasesDal purchasesDal;
        private readonly ILogger<PurchasesService> _logger;

        public PurchasesService(IPurchasesDal dal, ILogger<PurchasesService> logger)
        {
            purchasesDal = dal;
            _logger = logger;
        }

        public async Task<List<Ticket>> GetTicketsByGiftId(int giftId)
        {
            _logger.LogInformation("Fetching tickets for GiftId={GiftId}", giftId);
            var tickets = await purchasesDal.GetTicketsByGiftId(giftId);
            _logger.LogInformation("Found {Count} tickets for GiftId={GiftId}", tickets?.Count ?? 0, giftId);
            return tickets;
        }

        public async Task<List<Gift>> GetGiftsSortedByPrice()
        {
            _logger.LogInformation("Fetching gifts sorted by price");
            var gifts = await purchasesDal.GetGiftsSortedByPrice();
            _logger.LogInformation("Found {Count} gifts sorted by price", gifts?.Count ?? 0);
            return gifts;
        }

        public async Task<List<Gift>> GetGiftsSortedByPurchases()
        {
            _logger.LogInformation("Fetching gifts sorted by purchases");
            var gifts = await purchasesDal.GetGiftsSortedByPurchases();
            _logger.LogInformation("Found {Count} gifts sorted by purchases", gifts?.Count ?? 0);
            return gifts;
        }

        public async Task<List<User>> GetBuyersByGiftId(int giftId)
        {
            _logger.LogInformation("Fetching buyers for GiftId={GiftId}", giftId);
            var buyers = await purchasesDal.GetBuyersByGiftId(giftId);
            _logger.LogInformation("Found {Count} buyers for GiftId={GiftId}", buyers?.Count ?? 0, giftId);
            return buyers;
        }
    }
}
