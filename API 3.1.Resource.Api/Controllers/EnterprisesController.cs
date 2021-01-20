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
    public class EnterprisesController : ControllerBase
    {
        private readonly ProjectContext _context;

        public EnterprisesController(ProjectContext context)
        {
            _context = context;
        }

        // GET: api/Enterprises/getrooms/4
        [HttpGet]
        [Route("getRooms/{id}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<IEnumerable<Room>> GetEnterpriseRooms([FromRoute] int id)
        {
            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Enterprise> enterprises = GetUserElements.GetUserEnterprises(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (enterprises == null || enterprises.Count == 0 || enterprises.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            return _context.Rooms.Where(x => x.EnterpriseId == id).ToList();
        }

        // GET: api/Enterprises/getall
        [HttpGet]
        [Route("getAll")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<Enterprise> GetEnterprises()
        {
            return _context.Enterprises;
        }

        // GET: api/Enterprises/getbyid/5
        [HttpGet]
        [Route("getbyid/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetEnterprise([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Enterprise> enterprises = GetUserElements.GetUserEnterprises(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (enterprises == null || enterprises.Count == 0 || enterprises.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            var enterprise = await _context.Enterprises.FindAsync(id);

            if (enterprise == null)
            {
                return NotFound();
            }

            return Ok(enterprise);
        }

        // PUT: api/Enterprises/edit/5
        [HttpPut]
        [Route("edit/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PutEnterprise([FromRoute] int id, [FromBody] Enterprise enterprise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != enterprise.Id)
            {
                return BadRequest();
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Enterprise> enterprises = GetUserElements.GetUserEnterprises(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (enterprises == null || enterprises.Count == 0 || enterprises.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            _context.Entry(enterprise).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnterpriseExists(id))
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

        // POST: api/Enterprises/add
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PostEnterprise([FromBody] Enterprise enterprise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                if(Convert.ToInt32(User.FindFirstValue("Sub")) != enterprise.UserId)
                {
                    return BadRequest("Stop hacking pls...");
                }
            }

            if(GetUserElements.AddNewItem(enterprise.UserId, "Enterprise", _context) == false)
            {
                return BadRequest("Достигнуто максимально кол-во Enterprises для вашего тарифа.");
            }

            _context.Enterprises.Add(enterprise);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnterprise", new { id = enterprise.Id }, enterprise);
        }

        // DELETE: api/Enterprises/delete/5
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteEnterprise([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                List<Enterprise> enterprises = GetUserElements.GetUserEnterprises(Convert.ToInt32(User.FindFirstValue("Sub")), _context);
                if (enterprises == null || enterprises.Count == 0 || enterprises.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            List <Room> rooms = _context.Rooms.Where(x => x.EnterpriseId == id).ToList();

            List<T_sensor> t_Sensors = new List<T_sensor>();
            List<H_sensor> h_Sensors = new List<H_sensor>();
            foreach (Room room in rooms)
            {
                t_Sensors.AddRange(GetUserElements.GetUserT_sensorsFromRoom(room.Id, _context));
                h_Sensors.AddRange(GetUserElements.GetUserH_sensorsFromRoom(room.Id, _context));
            }

            if (t_Sensors != null && t_Sensors.Count > 0)
            {
                foreach (var t_s in t_Sensors)
                {
                    _context.T_sensors.Remove(t_s);
                    _context.Entry(t_s).State = EntityState.Deleted;
                    await _context.SaveChangesAsync();
                }
            }

            if (h_Sensors != null && h_Sensors.Count > 0)
            {
                foreach (var h_s in h_Sensors)
                {
                    _context.H_sensors.Remove(h_s);
                    _context.Entry(h_s).State = EntityState.Deleted;
                    await _context.SaveChangesAsync();
                }
            }

            if (rooms != null && rooms.Count > 0)
            {
                foreach (var room in rooms)
                {
                    _context.Rooms.Remove(room);
                    _context.Entry(room).State = EntityState.Deleted;
                    await _context.SaveChangesAsync();
                }
            }

            var enterprise = await _context.Enterprises.FindAsync(id);
            if (enterprise == null)
            {
                return NotFound();
            }

            _context.Enterprises.Remove(enterprise);
            _context.Entry(enterprise).State = EntityState.Deleted;

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool EnterpriseExists(int id)
        {
            return _context.Enterprises.Any(e => e.Id == id);
        }
    }
}
