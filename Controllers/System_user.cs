using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Quick_queue.Models;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Quick_queue.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/queue/{queueId}/system")]
    public class System_userController : ControllerBase
    {
        private readonly ILogger<System_userController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        public System_userController(QuickQueueContext context, ILogger<System_userController> logger, IJWTAuthenticationManager jWTAuthenticationManager)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
        }
        [Route("[controller]/get_queue")]
        [HttpGet]
        public IActionResult QueueGetUpdate([FromRoute] int queueId)
        {
            List<Queue> queue = _context.Queues.Where(qid => qid.EventId == queueId).ToList();
            return new OkObjectResult(queue);
        }

        [Route("/queue/system/enter/{EventId}")]
        [HttpPost]
        public IActionResult EnterQueue([FromRoute] Int64 EventId)
        {
            Queue new_position = new Queue();
            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            new_position.idUser = idUser;
            if (_context.Events.FirstOrDefault(e => e.EventId == EventId) is null) //чи існує івент
            {
                return NotFound();
            }
            if (_context.Events.FirstOrDefault(e => e.EventId == EventId).OwnerId == idUser) // хоче записатися у свою чергу
            {
                return Forbid();
            }
            if (_context.Queues.FirstOrDefault(u => u.idUser == idUser && u.EventId == EventId) is not null) //повторний запис?
            {
                return UnprocessableEntity();
            }
            new_position.EventId = EventId;
            List <Queue> queues = _context.Queues.Where(e => e.EventId == EventId).ToList();
            new_position.Number = queues.LastOrDefault() == null ? 1 : queues.Last().Number + 1;
            new_position.Time_queue = DateTime.UtcNow;
            _context.Add(new_position);
            _context.SaveChanges();
            return new OkObjectResult(new_position);
        }

        [Route("/queue/system/delete/{EventId}")]
        [HttpDelete]
        public IActionResult LeaveQueue([FromRoute] Int64 EventId)
        {
            Int64 idUser = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Queue deleted_queue = _context.Queues.FirstOrDefault(q => q.EventId == EventId && q.idUser == idUser);
            if (deleted_queue is null)
            {
                return NotFound();
            }
            _context.Remove(deleted_queue);
            _context.SaveChanges();
            return new OkResult();
        }
    }
}