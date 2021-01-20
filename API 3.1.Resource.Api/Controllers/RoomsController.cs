using API_3._1.Resource.Api.Classes;
using API_3._1.Resource.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_3._1.Resource.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ProjectContext _context;

        public RoomsController(ProjectContext context)
        {
            _context = context;
        }

        // GET: api/Rooms/get_t_sensors/{id}
        [HttpGet]
        [Route("get_t_sensors/{id}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<IEnumerable<T_sensor>> GetT_sensors([FromRoute] int id)
        {
            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Room> rooms = GetUserElements.GetUserRooms(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (rooms == null || rooms.Count == 0 || rooms.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }
            return _context.T_sensors.Where(x => x.RoomId == id).ToList();
        }

        // GET: api/Rooms/get_h_sensors/{id}
        [HttpGet]
        [Route("get_h_sensors/{id}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<IEnumerable<H_sensor>> GetH_sensors([FromRoute] int id)
        {
            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Room> rooms = GetUserElements.GetUserRooms(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (rooms == null || rooms.Count == 0 || rooms.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            return _context.H_sensors.Where(x => x.RoomId == id).ToList();
        }

        // GET: api/Rooms/getall
        [HttpGet]
        [Route("getall")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<Room> GetRooms()
        {
            return _context.Rooms;
        }

        // GET: api/Rooms/getbyid/5
        [HttpGet]
        [Route("getbyid/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetRoom([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Room> rooms = GetUserElements.GetUserRooms(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (rooms == null || rooms.Count == 0 || rooms.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // PUT: api/Rooms/edit/5
        [HttpPut]
        [Route("edit/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PutRoom([FromRoute] int id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != room.Id)
            {
                return BadRequest();
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Room> rooms = GetUserElements.GetUserRooms(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (rooms == null || rooms.Count == 0 || rooms.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Rooms/add
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PostRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                if (Convert.ToInt32(User.FindFirstValue("Sub")) != _context.Enterprises.Find(room.EnterpriseId).UserId)
                {
                    return BadRequest("Stop hacking pls...");
                }
            }

            if (GetUserElements.AddNewItem(_context.Enterprises.Find(room.EnterpriseId).UserId, "Room", _context) == false)
            {
                return BadRequest("Достигнуто максимально кол-во Rooms для вашего тарифа.");
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        // DELETE: api/Rooms/delete/5
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteRoom([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Room> rooms = GetUserElements.GetUserRooms(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (rooms == null || rooms.Count == 0 || rooms.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            List<T_sensor> t_Sensors = GetUserElements.GetUserT_sensorsFromRoom(id, _context);

            if (t_Sensors != null && t_Sensors.Count > 0)
            {
                foreach (var t_s in t_Sensors)
                {
                    _context.T_sensors.Remove(t_s);
                    _context.Entry(t_s).State = EntityState.Deleted;
                }
                await _context.SaveChangesAsync();
            }

            List<H_sensor> h_Sensors = GetUserElements.GetUserH_sensorsFromRoom(id, _context);

            if (h_Sensors != null && h_Sensors.Count > 0)
            {
                foreach (var h_s in h_Sensors)
                {
                    _context.H_sensors.Remove(h_s);
                    _context.Entry(h_s).State = EntityState.Deleted;
                }
                await _context.SaveChangesAsync();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
