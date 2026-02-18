using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Bll.Interfaces;
using server.Models;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "manager")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchasesService purchasesService;
        private readonly ILogger<PurchasesController> logger;

        public PurchasesController(IPurchasesService service, ILogger<PurchasesController> logger)
        {
            purchasesService = service;
            this.logger = logger;
        }

        [HttpGet("tickets-by-gift/{giftId}")]
        public async Task<IActionResult> GetTicketsByGift(int giftId)
        {
            logger.LogInformation("GetTicketsByGift started. GiftId={GiftId}", giftId);
            try
            {
                var result = await purchasesService.GetTicketsByGiftId(giftId);
                logger.LogInformation("GetTicketsByGift finished successfully. GiftId={GiftId} Count={Count}", giftId, result?.Count ?? 0);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetTicketsByGift. GiftId={GiftId}", giftId);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("gifts-by-price")]
        public async Task<IActionResult> GetGiftsByPrice()
        {
            logger.LogInformation("GetGiftsByPrice started");
            var result = await purchasesService.GetGiftsSortedByPrice();
            logger.LogInformation("GetGiftsByPrice finished successfully. Count={Count}", result?.Count ?? 0);

            return Ok(result);
        }
        [HttpGet("gifts-by-buyers")]
        public async Task<IActionResult> GetGiftsByBuyersNumber()
        {
            logger.LogInformation("GetGiftsByBuyersNumber started");

            var result = await purchasesService.GetGiftsSortedByPurchases();
            logger.LogInformation("GetGiftsByBuyersNumber finished successfully. Count={Count}", result?.Count ?? 0);

            return Ok(result);
        }

        [HttpGet("buyers-by-gift/{giftId}")]
        public async Task<IActionResult> GetBuyersByGift(int giftId)
        {
            logger.LogInformation("GetBuyersByGift started. GiftId={GiftId}", giftId);

            try
            {
                var result = await purchasesService.GetBuyersByGiftId(giftId);
                logger.LogInformation("GetBuyersByGift finished successfully. GiftId={GiftId} Count={Count}", giftId, result?.Count ?? 0);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetBuyersByGift. GiftId={GiftId}", giftId);
                return BadRequest(ex.Message);
            }
        }
    }
}