using API_3._1.Resource.Api.Classes;
using API_3._1.Resource.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_3._1.Resource.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class H_sensorController : ControllerBase
    {
        private readonly ProjectContext _context;

        public H_sensorController(ProjectContext context)
        {
            _context = context;
        }

        // GET: api/H_sensor/getall
        [HttpGet]
        [Route("getall")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<H_sensor> GetH_sensors()
        {
            return _context.H_sensors;
        }

        // GET: api/H_sensor/getbyid/5
        [HttpGet]
        [Route("getbyid/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetH_sensor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                User user = _context.Users.Find(Convert.ToInt32(User.FindFirstValue("Sub")));

                List<H_sensor> h_Sensors = GetUserElements.GetUserH_sensors(user.Id, _context);

                if (h_Sensors == null || h_Sensors.Count == 0 || h_Sensors.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            var h_sensor = await _context.H_sensors.FindAsync(id);

            if (h_sensor == null)
            {
                return NotFound();
            }

            return Ok(h_sensor);
        }

        // PUT: api/H_sensor/edit/5
        [HttpPut]
        [Route("edit/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PutH_sensor([FromRoute] int id, [FromBody] H_sensor h_sensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != h_sensor.Id)
            {
                return BadRequest();
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                User user = _context.Users.Find(Convert.ToInt32(User.FindFirstValue("Sub")));

                List<H_sensor> h_Sensors = GetUserElements.GetUserH_sensors(user.Id, _context);

                if (h_Sensors == null || h_Sensors.Count == 0 || h_Sensors.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            //Alarm
            H_sensor sensor = _context.H_sensors.Find(h_sensor.Id);
            if(h_sensor.Value < sensor.Min_value)
            {
                _context.Rooms.Find(h_sensor.RoomId).Alarm = true;

                Enterprise enterprise = _context.Enterprises.Find(_context.Rooms.Find(h_sensor.RoomId).EnterpriseId);
                User user = _context.Users.Find(enterprise.UserId);

                SendMail.sendEmail(user.Mail, "ALARM");
            }
            else if(h_sensor.Value > sensor.Max_value)
            {
                _context.Rooms.Find(h_sensor.RoomId).Alarm = true;

                Enterprise enterprise = _context.Enterprises.Find(_context.Rooms.Find(h_sensor.RoomId).EnterpriseId);
                User user = _context.Users.Find(enterprise.UserId);

                SendMail.sendEmail(user.Mail, "ALARM");
            }

            _context.Entry(h_sensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!H_sensorExists(id))
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

        // POST: api/H_sensor/add
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PostH_sensor([FromBody] H_sensor h_sensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                if (Convert.ToInt32(User.FindFirstValue("Sub")) != _context.Enterprises.Find(_context.Rooms.Find(h_sensor.RoomId).EnterpriseId).UserId)
                {
                    return BadRequest("Stop hacking pls...");
                }
            }

            if (GetUserElements.AddNewItem(Convert.ToInt32(User.FindFirstValue("Sub")), "Sensor", _context) == false)
            {
                return BadRequest("Достигнуто максимально кол-во Sensors для вашего тарифа.");
            }

            _context.H_sensors.Add(h_sensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetH_sensor", new { id = h_sensor.Id }, h_sensor);
        }

        // DELETE: api/H_sensor/delete/5
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteH_sensor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                User user = _context.Users.Find(Convert.ToInt32(User.FindFirstValue("Sub")));

                List<H_sensor> h_Sensors = GetUserElements.GetUserH_sensors(user.Id, _context);

                if (h_Sensors == null || h_Sensors.Count == 0 || h_Sensors.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            var h_sensor = await _context.H_sensors.FindAsync(id);
            if (h_sensor == null)
            {
                return NotFound();
            }

            _context.H_sensors.Remove(h_sensor);
            await _context.SaveChangesAsync();

            return Ok(h_sensor);
        }

        private bool H_sensorExists(int id)
        {
            return _context.H_sensors.Any(e => e.Id == id);
        }
    }
}
