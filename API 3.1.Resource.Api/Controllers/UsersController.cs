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
    public class UsersController : ControllerBase
    {
        private readonly ProjectContext _context;

        public UsersController(ProjectContext context)
        {
            _context = context;
        }

        // POST: api/Users/Authorization
        [HttpPost]
        [Route("Authorization")]
        [Authorize(Roles = "Admin, User")]
        public UserAuth Authorization([FromHeader(Name = "Authorization")] string token)
        {
            int user_id;
            Int32.TryParse(User.FindFirstValue("Sub"), out user_id);
            User user = _context.Users.Find(user_id);
            return new UserAuth()
            {
                Id = user_id,
                Mail = user.Mail,
                Name = user.Name,
                Surname = user.Surname,
                Role = _context.Roles.Find(user.RoleId).Name,
                Tariff = _context.Tariffs.Find(user.TariffId).Name
            };
        }

        // GET: api/Users/getEnterprises/2
        [HttpGet]
        [Route("getEnterprises/{id}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<IEnumerable<Enterprise>> GetUserEnterprises([FromRoute] int id)
        {
            if(User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                if(Convert.ToInt32(User.FindFirstValue("Sub")) != id)
                {
                    return BadRequest("id пользователя не соответствует запрашиваемому id.");
                }
            }
            return Ok(_context.Enterprises.Where(x => x.UserId == id).ToList());
        }

        // GET: api/Users
        [HttpGet]
        [Route("getall")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users;
        }

        // GET: api/Users/getbyid/5
        [HttpGet]
        [Route("getbyid/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/edit/5
        [HttpPut]
        [Route("edit/{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] ChangeProfile user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            if (User.Claims.Single(x => x.Type == ClaimTypes.Role).Value == "User")
            {
                if (Convert.ToInt32(User.FindFirstValue("Sub")) != id || Convert.ToInt32(User.FindFirstValue("Sub")) != user.Id)
                {
                    return BadRequest("id пользователя не соответствует запрашиваемому id.");
                }
            }

            //_context.Entry(user).State = EntityState.Modified;

            User current_user = _context.Users.Find(id);

            _context.Entry(current_user).Property(x => x.Name).CurrentValue = user.Name;
            _context.Entry(current_user).Property(x => x.Surname).CurrentValue = user.Surname;
            if (user.Pass.ToLower() != "empty")
            {
                _context.Entry(current_user).Property(x => x.Pass).CurrentValue = user.Pass;
                _context.Entry(current_user).Property(x => x.Name).IsModified = true;
            }

            _context.Entry(current_user).Property(x => x.Surname).IsModified = true;
            _context.Entry(current_user).Property(x => x.Pass).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users/add
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mail = new SqlParameter("mail", user.Mail);
            string sql = $"Select * From Users Where [Users].[Mail] = @mail";
            var exist = _context.Users.FromSqlRaw(sql,mail).ToList();
            if (exist.Count != 0)
            {
                //return Ok(exist);
                return BadRequest("Пользователь с таким электронным адресом уже существует в базе.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/delete/5
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
