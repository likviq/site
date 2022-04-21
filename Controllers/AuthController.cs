using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using Quick_queue.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Quick_queue.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        public AuthController(QuickQueueContext context, ILogger<AuthController> logger, IJWTAuthenticationManager jWTAuthenticationManager)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
        }

        [HttpPost("/Login")]
        public IActionResult Authenticate([FromBody] UserCredentials userCred)
        {
            var token = _JWTAuthenticationManager.Authenticate(userCred.Email, userCred.Password);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

        [HttpPost("/Register")]
        public IActionResult Register(User user)
        {
            bool registration_result = _JWTAuthenticationManager.Registration(user);
            if (!registration_result) // пошта зайнята іншим користувачем?
            {
                return new ConflictObjectResult("Email is already taken!");
            }
            return new OkObjectResult(user);
        }
    }
}
