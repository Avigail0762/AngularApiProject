using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using BCrypt.Net;



namespace server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly ILogger<PasswordController> logger;

        public PasswordController(ILogger<PasswordController> logger)
        {
            this.logger = logger;
        }

        // GET api/<PasswordController>/5
        [HttpGet("{pass}")]
        public string Get(string pass)
        {
            logger.LogInformation("Hash password request received");
            return HashPassword(pass);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
