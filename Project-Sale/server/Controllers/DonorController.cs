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
    public class DonorController : ControllerBase
    {
        private readonly IDonorService donorService;
        private readonly ILogger<DonorController> logger;

        public DonorController(IDonorService donorService, ILogger<DonorController> logger)
        {
            this.donorService = donorService;
            this.logger = logger;
        }

        // GET: api/donor
        [HttpGet]
        public async Task<ActionResult<List<Donor>>> Get()
        {
            logger.LogInformation("Get all donors started");

            var donors = await donorService.Get();

            if (donors == null || donors.Count == 0)
            {
                logger.LogWarning("No donors found");
                return NoContent();
            }

            logger.LogInformation("Get all donors finished. Count={Count}", donors.Count);
            return Ok(donors);
        }

        // GET: api/donor/email/{email}
        [HttpGet("email/{email}")]
        public async Task<ActionResult<Donor>> GetByEmail(string email)
        {
            logger.LogInformation("Get donor by email started. Email={Email}", email);

            if (string.IsNullOrEmpty(email))
            {
                logger.LogWarning("Empty email received");
                return BadRequest("Email is required");
            }

            var donor = await donorService.GetByEmail(email);

            if (donor == null)
            {
                logger.LogWarning("Donor not found. Email={Email}", email);
                return NotFound($"Donor with email '{email}' not found");
            }

            logger.LogInformation("Get donor by email finished successfully. Email={Email}", email);
            return Ok(donor);
        }

        // GET: api/donor/name?firstName=...&lastName=...
        [HttpGet("name")]
        public async Task<ActionResult<Donor>> GetByName(
            [FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            logger.LogInformation("Get donor by name started. FirstName={FirstName}, LastName={LastName}",
                 firstName, lastName);

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                logger.LogWarning("Invalid donor name parameters");
                return BadRequest("First name and last name are required");
            }

            var donor = await donorService.GetByName(firstName, lastName);
            if (donor == null)
            {
                logger.LogWarning("Donor not found. FirstName={FirstName}, LastName={LastName}",
                    firstName, lastName);
                return NotFound("Donor not found");
            }

            logger.LogInformation("Get donor by name finished successfully");
            return Ok(donor);
        }

        // POST: api/donor
        [HttpPost]
        public async Task<ActionResult<Donor>> Add([FromBody] DonorDTO donor)
        {
            logger.LogInformation("Add donor started");

            if (donor == null)
            {
                logger.LogWarning("Donor data is null");
                return BadRequest("Donor data is required");
            }

            try
            {
                logger.LogDebug("Calling DonorService.Add");

                var newDonor = await donorService.Add(donor);

                logger.LogInformation("Donor added successfully. Email={Email}", newDonor.Email);

                return CreatedAtAction(
                    nameof(GetByEmail),
                    new { email = newDonor.Email },
                    newDonor
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding donor");
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/donor/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DonorDTO donor)
        {
            logger.LogInformation("Update donor started. Id={Id}", id);

            if (id <= 0 || donor == null)
            {
                logger.LogWarning("Invalid update parameters. Id={Id}", id);
                return BadRequest("Invalid donor data");
            }

            try
            {
                await donorService.Update(id, donor);
                logger.LogInformation("Donor updated successfully. Id={Id}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Donor not found for update. Id={Id}", id);
                return NotFound($"Donor with id {id} not found");
            }
        }

        // DELETE: api/donor/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            logger.LogInformation("Remove donor started. Id={Id}", id);

            if (id <= 0)
            {
                logger.LogWarning("Invalid donor id for remove. Id={Id}", id);
                return BadRequest("Invalid donor id");
            }

            var result = await donorService.Remove(id);
            if (!result)
            {
                logger.LogWarning("Donor not found for remove. Id={Id}", id);
                return NotFound($"Donor with id {id} not found");
            }

            logger.LogInformation("Donor removed successfully. Id={Id}", id);

            return Ok();
        }
    }

}
