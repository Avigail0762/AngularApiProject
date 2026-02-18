using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Bll.Interfaces;
using server.Models;
using server.Models.DTO;


namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "manager")]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService giftService;
        private readonly ILogger<GiftController> logger;

        public GiftController(IGiftService giftService, ILogger<GiftController> logger)
        {
            this.giftService = giftService;
            this.logger = logger;
        }

        // GET: api/gift
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Gift>>> Get()
        {
            logger.LogInformation("Get all gifts started");

            var gifts = await giftService.Get();

            if (gifts == null || gifts.Count == 0)
            {
                logger.LogWarning("No gifts found");
                return NoContent();
            }
            logger.LogInformation("Get all gifts finished. Count={Count}", gifts.Count);
            return Ok(gifts);
        }

        // GET: api/gift/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Gift>> GetById(int id)
        {
            logger.LogInformation("Get gift by id started. Id={Id}", id);

            if (id <= 0)
            {
                logger.LogWarning("Invalid gift id received. Id={Id}", id);
                return BadRequest("Invalid gift id");
            }

            var gift = await giftService.GetById(id);
            if (gift == null)
            {
                logger.LogWarning("Gift not found. Id={Id}", id);
                return NotFound($"Gift with id {id} not found");
            }
            logger.LogInformation("Get gift by id finished successfully. Id={Id}", id);
            return Ok(gift);
        }

        // GET: api/gift/name/{name}
        [HttpGet("name/{name}")]
        [AllowAnonymous]
        public async Task<ActionResult<Gift>> GetByName(string name)
        {
            logger.LogInformation("Get gift by name started. Name={Name}", name);

            if (string.IsNullOrEmpty(name))
            {
                logger.LogWarning("Empty gift name received");
                return BadRequest("Gift name is required");
            }

            var gift = await giftService.GetByName(name);
            if (gift == null)
            {
                logger.LogWarning("Gift not found by name. Name={Name}", name);
                return NotFound("Gift not found");
            }
            logger.LogInformation("Get gift by name finished successfully. Name={Name}", name);
            return Ok(gift);
        }

        // GET: api/gift/category/{category}
        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Gift>>> GetByCategory(string category)
        {
            logger.LogInformation("Get gifts by category started. Category={Category}", category);

            if (string.IsNullOrEmpty(category))
            {
                logger.LogWarning("Empty category received");
                return BadRequest("Category is required");
            }

            var gifts = await giftService.GetByCategory(category);
            if (gifts == null || gifts.Count == 0)
            {
                logger.LogWarning("No gifts found for category {Category}", category);
                return NoContent();
            }

            logger.LogInformation("Get gifts by category finished. Count={Count}", gifts.Count);
            return Ok(gifts);
        }

        // GET: api/gift/donor?firstName=...&lastName=...
        [HttpGet("donor")]
        public async Task<ActionResult<List<Gift>>> GetByDonorName(
            [FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            logger.LogInformation("Get gifts by donor started. FirstName={FirstName}, LastName={LastName}",
             firstName, lastName);
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                logger.LogWarning("Invalid donor name parameters");
                return BadRequest("First name and last name are required");
            }

            var gifts = await giftService.GetByDonorName(firstName, lastName);
            if (gifts == null || gifts.Count == 0)
            {
                logger.LogWarning("No gifts found for donor {FirstName} {LastName}", firstName, lastName);
                return NoContent();
            }

            logger.LogInformation("Get gifts by donor finished. Count={Count}", gifts.Count);
            return Ok(gifts);
        }

        // GET: api/gift/buyers/{number}
        [HttpGet("buyers/{number}")]
        public async Task<ActionResult<List<Gift>>> GetByBuyersNumber(int number)
        {
            if (number < 0)
                return BadRequest("Invalid buyers number");

            var gifts = await giftService.GetByBuyersNumber(number);
            if (gifts == null || gifts.Count == 0)
                return NoContent();

            return Ok(gifts);
        }

        // POST: api/gift
        [HttpPost]
        public async Task<ActionResult<Gift>> Add([FromBody] GiftDTO gift)
        {
            logger.LogInformation("Add gift started");
            if (gift == null)
            {
                logger.LogWarning("Gift data is null");
                return BadRequest("Gift data is required");
            }
            try
            {
                logger.LogDebug("Calling GiftService.Add");

                var newGift = await giftService.Add(gift);
                logger.LogInformation("Gift added successfully. Id={Id}", newGift.Id);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = newGift.Id },
                    newGift
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding gift");
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/gift/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GiftDTO gift)
        {
            logger.LogInformation("Update gift started. Id={Id}", id);

            if (id <= 0 || gift == null)
            {
                logger.LogWarning("Invalid update parameters. Id={Id}", id);
                return BadRequest("Invalid input");
            }

            try
            {
                await giftService.Update(id, gift);
                logger.LogInformation("Gift updated successfully. Id={Id}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Gift not found for update. Id={Id}", id);
                return NotFound($"Gift with id {id} not found");
            }
        }

        // DELETE: api/gift/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            logger.LogInformation("Remove gift started. Id={Id}", id);

            if (id <= 0)
            {
                logger.LogWarning("Invalid gift id for remove. Id={Id}", id);
                return BadRequest("Invalid gift id");
            }

            var result = await giftService.Remove(id);
            if (!result)
            {
                logger.LogWarning("Gift not found for remove. Id={Id}", id);
                return NotFound($"Gift with id {id} not found");
            }

            logger.LogInformation("Gift removed successfully. Id={Id}", id);
            return Ok();
        }
        // GET: api/gift/sorted?ascending=true
        [HttpGet("sorted")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Gift>>> GetByPrice([FromQuery] bool ascending = true)
        {
            logger.LogInformation("Get gifts sorted by price started. Ascending={Ascending}", ascending);
            var gifts = await giftService.GetByPrice(ascending);

            if (gifts == null || gifts.Count == 0)
            {
                logger.LogWarning("No gifts found for sorted price");
                return NoContent();
            }

            logger.LogInformation("Get gifts sorted by price finished. Count={Count}", gifts.Count);
            return Ok(gifts);
        }

    }
}