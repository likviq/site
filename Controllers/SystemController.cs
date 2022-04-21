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
    public class SystemController : Controller
    {
        private readonly ILogger<SystemController> _logger;

        private QuickQueueContext _context;

        private readonly IJWTAuthenticationManager _JWTAuthenticationManager;

        public SystemController(QuickQueueContext context, ILogger<SystemController> logger, IJWTAuthenticationManager jWTAuthenticationManager)
        {
            _context = context;
            _logger = logger;
            _JWTAuthenticationManager = jWTAuthenticationManager;
        }

        [Authorize]
        [Route("/queue/create/system/create")]
        [HttpPost]
        public IActionResult PostCreateEvent([FromBody] Event Event)
        {
            Int64 OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Event.OwnerId = OwnerId;
            // Без Title
            if (Event.Title == null) return BadRequest();
            // перевірити чи всі поля правильні
            _context.Add(Event);
            _context.SaveChanges();
            return new OkObjectResult(Event);
        }

        [Authorize]
        [Route("/queue/{idEvent}/moder/system/delete")]
        [HttpDelete]
        public IActionResult QueueIdModerSystemDelete([FromRoute] Int64 idEvent, [FromQuery] Int64 idUser)
        {
            Int64 OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Queue queue = _context.Queues.FirstOrDefault(c => c.idUser == idUser && c.EventId == idEvent);
            Event evnt = _context.Events.FirstOrDefault(c => c.EventId == idEvent);
            // Чи в токені лежить id модератора
            if (evnt == null || evnt.OwnerId != OwnerId) return Forbid();
            if (evnt.OwnerId != OwnerId) return UnprocessableEntity();
            // Перевіряю чи є така черга взагалі для того, щоб видалити
            try
            {
                _context.Remove(queue);
                return new OkObjectResult(queue);
            }
            catch
            {
                return NotFound();
            }
            _context.SaveChanges();
            return new OkObjectResult(queue);
        }

        [Authorize]
        [Route("/queue/{idEvent}/moder/system/update/")]
        [HttpPut]
        public IActionResult QueueIdModerSystemUpdate([FromRoute] Int64 idEvent, [FromBody] Event ev)
        {
            Int64 OwnerId = System.Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Event evnt = _context.Events.FirstOrDefault(c => c.EventId == idEvent);
            // перевіряю наявність потрібних даних
            try
            {
                evnt.Title = ev.Title;
            }
            catch
            {
                return NotFound();
            }
            if (evnt.OwnerId != OwnerId)
            {
                return Forbid();
            }
            try
            {
                _context.Update(evnt);
            }
            catch
            {
                return NotFound();
            }
            try
            {
                _context.SaveChanges();
            }
            catch
            {
                return UnprocessableEntity();
            }
            return new OkObjectResult(evnt);
        }
        // юра
        public class Comand
        {
            public byte next { get; set; }
            public byte open { get; set; }
            public byte close { get; set; }
            public byte finish { get; set; }
        }
        private Queue nextuser(int eventid)
        {
            Queue change = _context.Queues.Where(o => o.Status != 1 && o.EventId == eventid).OrderBy(o => o.Number).FirstOrDefault();
            // чи є користувачі в черзі для пропуску
            if (change == null)
                return null;//"nobody is waiting on queue";
            change.Status = 1;
            _context.SaveChanges();
            return change;//"Ok";
        }
        private bool close(Event Event)
        {
            // чи вже закрита 
            if (!Event.isSuspended)
                return false;

            Event.isSuspended = true;
            _context.SaveChanges();
            return true;
        }
        private bool finish(Event Event)
        {
            // поідеї тут каскадне видалення 
            _context.Remove(Event);
            _context.SaveChanges();
            return true;
        }

        private bool open(Event Event)
        {
            // чи вже відкрита 
            if (Event.isSuspended)
                return false;

            Event.isSuspended = false;
            _context.SaveChanges();
            return true;
        }

        [Authorize]
        [Route("/queue/{eventId}/moder/system/")]
        [HttpPost]
        public IActionResult ComandQueue([FromRoute] int eventId, [FromBody] Comand comand)
        {
            // чи існує таткак черга 
            Event Event = _context.Events.FirstOrDefault(c => c.EventId == eventId);
            if (Event == null)
                return NotFound();

            // чи адмін редагує чергу 
            long Admin = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (Event.OwnerId != Admin)
            {
                return Forbid();
            }
            if (comand.next != null) // 
            {
                var resalt = nextuser(eventId);
                if (resalt == null)
                    return BadRequest();
                else
                    return new OkObjectResult(resalt);
            }
            if (comand.close != null)
            {

                if (close(Event))
                    return new OkResult();
                else
                    return BadRequest();
            }
            if (comand.open != null)
            {
                if (close(Event))
                    return new OkResult();
                else
                    return BadRequest(":(((");
            }
            if (comand.finish != null)
            {
                if (finish(Event))
                    return new OkResult();
                else
                    return BadRequest(":(((");
            }
            return BadRequest(":(((");
        }
    }
}
