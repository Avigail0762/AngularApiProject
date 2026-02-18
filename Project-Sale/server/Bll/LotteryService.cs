using server.Bll.Interfaces;
using server.Dal.Interfaces;
using server.Models;
using server.Models.DTO;

namespace server.Bll
{
    public class LotteryService : ILotteryService
    {
        private readonly IGiftDal _giftDal;
        private readonly IPurchasesDal _purchasesDal;
        private readonly IEmailService _emailService;
        private readonly ILogger<LotteryService> _logger;

        public LotteryService(
            IGiftDal giftDal,
            IPurchasesDal purchasesDal,
            IEmailService emailService,
            ILogger<LotteryService> logger)
        {
            _giftDal = giftDal;
            _purchasesDal = purchasesDal;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Ticket> DoLottery(int giftId)
        {
            _logger.LogInformation("Lottery started for GiftId={GiftId}", giftId);

            var gift = await _giftDal.GetById(giftId);
            if (gift == null)
            {
                _logger.LogWarning("Gift not found. GiftId={GiftId}", giftId);
                throw new Exception("Gift not found");
            }

            if (gift.IsDrawn)
            {
                _logger.LogWarning("Lottery already done for this gift. GiftId={GiftId}", giftId);
                throw new Exception("Lottery already done for this gift");
            }

            var tickets = await _purchasesDal.GetTicketsByGiftId(giftId);
            if (tickets == null || tickets.Count == 0)
            {
                _logger.LogWarning("No tickets for this gift. GiftId={GiftId}", giftId);
                throw new Exception("No tickets for this gift");
            }

            // הכנת רשימת הגרלה לפי Quantity
            var ticketsIds = new List<int>();
            foreach (var ticket in tickets)
                for (int i = 0; i < ticket.Quantity; i++)
                    ticketsIds.Add(ticket.Id);

            var random = new Random();
            int winnerTicketId = ticketsIds[random.Next(ticketsIds.Count)];

            var winnerTicket = tickets.First(t => t.Id == winnerTicketId);

            // עדכון המתנה
            gift.WinnerTicketId = winnerTicket.Id;
            gift.IsDrawn = true;

            await _giftDal.Update(gift.Id, new GiftDTO
            {
                Name = gift.Name,
                Category = gift.Category,
                Price = gift.Price,
                BuyersNumber = gift.BuyersNumber,
                DonorId = gift.DonorId,
                WinnerTicketId = gift.WinnerTicketId,
                IsDrawn = gift.IsDrawn
            });

            _logger.LogInformation("Lottery completed. GiftId={GiftId}, WinnerTicketId={TicketId}", giftId, winnerTicket.Id);

            // שליחת מייל לזוכה
            var buyers = await _purchasesDal.GetBuyersByGiftId(gift.Id);
            var winnerUser = buyers.FirstOrDefault(u => u.Id == winnerTicket.UserId);

            if (winnerUser != null)
            {
                try
                {
                    _logger.LogInformation("Sending winner email. UserId={UserId}, Email={Email}", winnerUser.Id, winnerUser.Email);
                    await _emailService.SendWinnerEmailAsync(winnerUser.Email, gift.Name);
                    _logger.LogInformation("Winner email sent successfully. UserId={UserId}", winnerUser.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send winner email. UserId={UserId}", winnerUser.Id);
                }
            }

            return winnerTicket;
        }

        public async Task<List<Ticket>> GetWinnersReport()
        {
            _logger.LogInformation("Generating winners report started");
            var gifts = await _giftDal.Get();
            var winners = new List<Ticket>();

            foreach (var gift in gifts)
            {
                if (gift.IsDrawn && gift.WinnerTicketId.HasValue)
                {
                    var tickets = await _purchasesDal.GetTicketsByGiftId(gift.Id);
                    var ticket = tickets.FirstOrDefault(t => t.Id == gift.WinnerTicketId.Value);
                    if (ticket != null)
                        winners.Add(ticket);
                }
            }

            _logger.LogInformation("Winners report generated. Count={Count}", winners.Count);
            return winners;
        }

        public async Task<decimal> GetTotalIncome()
        {
            _logger.LogInformation("Calculating total income started");
            var gifts = await _giftDal.Get();
            decimal total = 0;

            foreach (var gift in gifts)
                total += gift.Price * gift.BuyersNumber;

            _logger.LogInformation("Total income calculated. Amount={Total}", total);
            return total;
        }
    }
}
