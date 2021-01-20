using API_3._1.Resource.Api.Classes;
using API_3._1.Resource.Api.ConnectString;
using API_3._1.Resource.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
    public class T_sensorController : ControllerBase
    {
        private readonly ProjectContext _context;

        public T_sensorController(ProjectContext context)
        {
            _context = context;
        }

        // GET: api/T_sensor/getall
        [HttpGet]
        [Route("getall")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<T_sensor> GetT_sensors()
        {
            return _context.T_sensors;
        }

        // GET: api/T_sensor/getbyid/5
        [HttpGet]
        [Route("getbyid/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetT_sensor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                User user = _context.Users.Find(Convert.ToInt32(User.FindFirstValue("Sub")));

                List<T_sensor> t_Sensors = GetUserElements.GetUserT_sensors(user.Id, _context);

                if (t_Sensors == null || t_Sensors.Count == 0 || t_Sensors.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            var t_sensor = await _context.T_sensors.FindAsync(id);

            if (t_sensor == null)
            {
                return NotFound();
            }

            return Ok(t_sensor);
        }

        // PUT: api/T_sensor/edit/5
        [HttpPut]
        [Route("edit/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PutT_sensor([FromRoute] int id, [FromBody] T_sensor t_sensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != t_sensor.Id)
            {
                return BadRequest();
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                User user = _context.Users.Find(Convert.ToInt32(User.FindFirstValue("Sub")));

                List<T_sensor> t_Sensors = GetUserElements.GetUserT_sensors(user.Id, _context);

                if (t_Sensors == null || t_Sensors.Count == 0 || t_Sensors.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            //Alarm
            T_sensor sensor = _context.T_sensors.Find(t_sensor.Id);
            if (t_sensor.Value < sensor.Min_value)
            {
                _context.Rooms.Find(t_sensor.RoomId).Alarm = true;

                Enterprise enterprise = _context.Enterprises.Find(_context.Rooms.Find(t_sensor.RoomId).EnterpriseId);
                User user = _context.Users.Find(enterprise.UserId);

                SendMail.sendEmail(user.Mail, "ALARM");
            }
            else if (t_sensor.Value > sensor.Max_value)
            {
                _context.Rooms.Find(t_sensor.RoomId).Alarm = true;

                Enterprise enterprise = _context.Enterprises.Find(_context.Rooms.Find(t_sensor.RoomId).EnterpriseId);
                User user = _context.Users.Find(enterprise.UserId);

                SendMail.sendEmail(user.Mail, "ALARM");
            }

            _context.Entry(t_sensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!T_sensorExists(id))
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

        // POST: api/T_sensor/add
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PostT_sensor([FromBody] T_sensor t_sensor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                if (Convert.ToInt32(User.FindFirstValue("Sub")) != _context.Enterprises.Find(_context.Rooms.Find(t_sensor.RoomId).EnterpriseId).UserId)
                {
                    return BadRequest("Stop hacking pls...");
                }
            }

            if (GetUserElements.AddNewItem(Convert.ToInt32(User.FindFirstValue("Sub")), "Sensor", _context) == false)
            {
                return BadRequest("Достигнуто максимально кол-во Sensors для вашего тарифа.");
            }

            _context.T_sensors.Add(t_sensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetT_sensor", new { id = t_sensor.Id }, t_sensor);
        }

        // DELETE: api/T_sensor/delete/5
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> DeleteT_sensor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                User user = _context.Users.Find(Convert.ToInt32(User.FindFirstValue("Sub")));

                List<T_sensor> t_Sensors = GetUserElements.GetUserT_sensors(user.Id, _context);

                if (t_Sensors == null || t_Sensors.Count == 0 || t_Sensors.Where(x => x.Id == id).ToList().Count == 0)
                {
                    return NotFound();
                }
            }

            var t_sensor = await _context.T_sensors.FindAsync(id);
            if (t_sensor == null)
            {
                return NotFound();
            }

            _context.T_sensors.Remove(t_sensor);
            await _context.SaveChangesAsync();

            return Ok(t_sensor);
        }

        private bool T_sensorExists(int id)
        {
            return _context.T_sensors.Any(e => e.Id == id);
        }
    }
}
