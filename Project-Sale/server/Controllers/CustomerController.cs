//using Micserver\Controllers\CustomerController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Bll.Interfaces;
using server.Models.DTO;

namespace server.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerService customerService,
            ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        // ---------- AUTH ----------

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO dto)
        {
            _logger.LogInformation("Customer register started");

            if (dto == null)
            {
                _logger.LogWarning("Register failed - UserDTO is null");
                return BadRequest("User data is required");
            }

            try
            {
                var user = await _customerService.Register(dto);

                _logger.LogInformation("Customer registered successfully. Email={Email}", dto.Email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during customer registration");
                return BadRequest(ex.Message);
            }
        }


        // ---------- GIFTS ----------

        [HttpGet("gifts")]
        public async Task<IActionResult> GetGifts(
            [FromQuery] string? category,
            [FromQuery] bool? sortPriceAsc)
        {
            _logger.LogInformation(
               "Get gifts started. Category={Category}, SortPriceAsc={SortPriceAsc}",
               category, sortPriceAsc);

            var gifts = await _customerService.GetGifts(category, sortPriceAsc);

            _logger.LogInformation("Get gifts finished successfully. Count={Count}", gifts.Count);
            return Ok(gifts);
        }

        // ---------- CART ----------

        [HttpPost("cart/add")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddToCart(
            [FromQuery] int userId,
            [FromQuery] int giftId)
        {
            _logger.LogInformation("Add to cart started. UserId={UserId}, GiftId={GiftId}", userId, giftId);

            await _customerService.AddToCart(userId, giftId);
            _logger.LogInformation("Gift added to cart successfully");
            return Ok();
        }

        [HttpDelete("cart/remove")]
        [Authorize(Roles = "user")]

        public async Task<IActionResult> RemoveFromCart(
            [FromQuery] int userId,
            [FromQuery] int giftId)
        {
            _logger.LogInformation("Remove from cart started. UserId={UserId}, GiftId={GiftId}", userId, giftId);

            await _customerService.RemoveFromCart(userId, giftId);
            _logger.LogInformation("Gift removed from cart successfully");
            return Ok();
        }

        [HttpGet("cart")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetCart([FromQuery] int userId)
        {
            _logger.LogInformation("Get cart started. UserId={UserId}", userId);

            if (userId <= 0)
            {
                _logger.LogWarning("Get cart failed - invalid userId. UserId={UserId}", userId);
                return BadRequest("Invalid user id");
            }

            try
            {
                var cart = await _customerService.GetCart(userId);

                _logger.LogInformation("Get cart completed successfully. UserId={UserId}, Count={Count}", userId, cart.Count);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting cart. UserId={UserId}", userId);
                return BadRequest(ex.Message);
            }
        }

        // ---------- PURCHASE ----------

        [HttpPost("purchase")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Purchase([FromQuery] int userId)
        {
            _logger.LogInformation("Purchase started. UserId={UserId}", userId);

            if (userId <= 0)
            {
                _logger.LogWarning("Invalid userId for purchase. UserId={UserId}", userId);
                return BadRequest("Invalid user id");
            }

            try
            {
                _logger.LogDebug("Calling Purchase service");

                await _customerService.Purchase(userId);

                _logger.LogInformation("Purchase completed successfully. UserId={UserId}", userId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during purchase. UserId={UserId}", userId);
                return BadRequest(ex.Message);
            }
        }
    }
}