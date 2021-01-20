using API_3._1.Resource.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_3._1.Resource.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TariffsController : ControllerBase
    {
        private readonly ProjectContext _context;

        public TariffsController(ProjectContext context)
        {
            _context = context;
        }

        // GET: api/Tariffs/getall
        [HttpGet]
        [Route("getall")]
        [Authorize(Roles = "Admin, User")]
        public IEnumerable<Tariff> GetTariffs()
        {
            return _context.Tariffs;
        }

        // GET: api/Tariffs/getbyid/5
        [HttpGet]
        [Route("getbyid/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetTariff([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tariff = await _context.Tariffs.FindAsync(id);

            if (tariff == null)
            {
                return NotFound();
            }

            return Ok(tariff);
        }

        // PUT: api/Tariffs/edit/5
        [HttpPut]
        [Route("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTariff([FromRoute] int id, [FromBody] Tariff tariff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tariff.Id)
            {
                return BadRequest();
            }

            _context.Entry(tariff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TariffExists(id))
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

        // POST: api/Tariffs/add
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostTariff([FromBody] Tariff tariff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var name = new SqlParameter("name", tariff.Name);
            var max_enterprises = new SqlParameter("max_enterprises", tariff.Max_enterprises);
            var max_rooms = new SqlParameter("max_rooms", tariff.Max_rooms);
            var max_sensors = new SqlParameter("max_sensors", tariff.Max_sensors);
            var price = new SqlParameter("price", tariff.Price);

            string str = $"Select * From Tariffs Where [Tariffs].[Name] = @name and " +
                $"[Tariffs].[Max_enterprises] = @max_enterprises and " +
                $"[Tariffs].[Max_rooms] = @max_rooms and " +
                $"[Tariffs].[Max_sensors] = @max_sensors and " +
                $"[Tariffs].[Price] = @price";

            var exist = _context.Tariffs.FromSqlRaw(str, name, max_enterprises, max_rooms, max_sensors, price).ToList();
            if (exist.Count != 0)
            {
                return Ok(exist);
            }

            _context.Tariffs.Add(tariff);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTariff", new { id = tariff.Id }, tariff);
        }

        // DELETE: api/Tariffs/delete/5
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTariff([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tariff = await _context.Tariffs.FindAsync(id);
            if (tariff == null)
            {
                return NotFound();
            }

            _context.Tariffs.Remove(tariff);
            await _context.SaveChangesAsync();

            return Ok(tariff);
        }

        private bool TariffExists(int id)
        {
            return _context.Tariffs.Any(e => e.Id == id);
        }
    }
}
