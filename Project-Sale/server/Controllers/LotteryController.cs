using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Bll.Interfaces;
using server.Models;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "manager")]
    public class LotteryController : ControllerBase
    {
        private readonly ILotteryService _lotteryService;
        private readonly ILogger<LotteryController> _logger;

        public LotteryController(ILotteryService lotteryService, ILogger<LotteryController> logger)
        {
            _lotteryService = lotteryService;
            _logger = logger;
        }

        // POST: api/lottery/draw/{giftId}
        // מבצע הגרלה על מתנה ספציפית ומחזיר את הכרטיס המנצח
        [HttpPost("draw/{giftId}")]
        public async Task<ActionResult<Ticket>> Draw(int giftId)
        {
            _logger.LogInformation("Lottery draw started. GiftId={GiftId}", giftId);

            try
            {
                _logger.LogDebug("Calling DoLottery service");

                var winnerTicket = await _lotteryService.DoLottery(giftId);
                _logger.LogInformation("Lottery draw finished successfully. GiftId={GiftId} WinnerTicketId={WinnerId}", giftId, winnerTicket?.Id);

                return Ok(winnerTicket);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during lottery draw. GiftId={GiftId}", giftId);
                return BadRequest(ex.Message);
            }
        }

        // GET: api/lottery/winners
        // מחזיר את כל הזוכים לכל המתנות
        [HttpGet("winners")]
        public async Task<ActionResult<List<Ticket>>> GetWinners()
        {
            _logger.LogInformation("GetWinners request started");
            var winners = await _lotteryService.GetWinnersReport();
            if (winners == null || winners.Count == 0)
            {
                _logger.LogWarning("No winners found");
                return NoContent();
            }
            _logger.LogInformation("GetWinners finished successfully. Count={Count}", winners.Count);

            return Ok(winners);
        }

        // GET: api/lottery/total-income
        // מחזיר את סך ההכנסות מכל הרכישות
        [HttpGet("total-income")]
        public async Task<ActionResult<decimal>> GetTotalIncome()
        {
            _logger.LogInformation("GetTotalIncome request started");

            var totalIncome = await _lotteryService.GetTotalIncome();

            _logger.LogInformation("GetTotalIncome finished successfully. TotalIncome={TotalIncome}", totalIncome);
            return Ok(totalIncome);
        }

    }

}