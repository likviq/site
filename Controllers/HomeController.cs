using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quick_queue.Models;
using Microsoft.EntityFrameworkCore;
using Quick_queue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Quick_queue.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        public HomeController(QuickQueueContext context, ILogger<HomeController> logger, IJWTAuthenticationManager jWTAuthenticationManager)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
        }

        [Authorize]
        [Route("/123")]
        [HttpGet]
        public IActionResult Get()
        {
            var userId = User.FindFirst(ClaimTypes.Email).Value;
            //List<User> user = _context.Users.Where(c => c.idUser > 7).ToList();
            return new OkObjectResult(userId);
        }

        [Authorize]
        [Route("/1234")]
        [HttpGet]
        public IActionResult GetAgain()
        {
            var userId = User.FindFirst(ClaimTypes.Email).Value;
            List<Queue> queue = _context.Queues.Where(c => c.EventId == 3).ToList();
            List<User> users = new List<User>();
            foreach (Queue i in queue)
            {
                User user = _context.Users.First(c => c.idUser == i.idUser);
                users.Add(user);
            }
            return new OkObjectResult(users);
        }

        [HttpPut]
        public IActionResult PutUser(User user)
        {
            return new OkObjectResult(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateUser(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
            return new OkObjectResult(user);
        }

        [Route("/event")]
        [HttpPost]
        public IActionResult CreateEvent(Event event1)
        {
            _context.Add(event1);
            _context.SaveChanges();
            return new OkObjectResult(event1);
        }
    }
}