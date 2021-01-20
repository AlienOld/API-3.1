using API_3._1.Auth.Api.Classes;
using API_3._1.Auth.Api.Models;
using API_3._1.Auth.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_3._1.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> authOptions;
        SqlConnection connection;

        public AuthController(IOptions<AuthOptions> authOptions)
        {
            this.authOptions = authOptions;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] Login request)
        {
            var user = Authenticate(request.Email, request.Password);

            if (user != null)
            {
                var token = GenerateJWT(user);

                return Ok(new
                {
                    access_token = token
                });
            }

            return Unauthorized("Wrong email or password.");
        }

        private Account Authenticate(string email, string pass)
        {
            Account account = null;

            if (connection == null || connection.State == ConnectionState.Closed)
            {
                connection = new SqlConnection(ConnectionString.str);
                try
                {
                    connection.Open();
                }
                catch
                {
                    return null;
                }
            }

            SqlParameter mail = new SqlParameter("Email", SqlDbType.NVarChar);
            SqlParameter password = new SqlParameter("Pass", SqlDbType.NVarChar);

            SqlCommand getUser = new SqlCommand("Select * From Users Where Mail = @Email and Pass = @Pass", connection);

            getUser.Parameters.AddRange(new SqlParameter[] { mail, password });

            getUser.Parameters["Email"].SqlValue = email;
            getUser.Parameters["Pass"].SqlValue = pass;

            SqlDataReader reader = getUser.ExecuteReader();
            int rl_id = 0;

            while (reader.Read())
            {
                account = new Account()
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Mail = reader["Mail"].ToString(),
                    Pass = reader["Pass"].ToString(),
                    Name = reader["Name"].ToString(),
                    Surname = reader["Surname"].ToString(),
                    TariffId = Convert.ToInt32(reader["TariffId"])
                };

                rl_id = Convert.ToInt32(reader["RoleId"]);
            }

            if (reader != null && !reader.IsClosed)
                reader.Close();

            if (account != null)
            {
                SqlParameter role_id = new SqlParameter("RoleId", SqlDbType.Int);
                SqlCommand get_role = new SqlCommand("Select Name from Roles Where Id = @RoleId", connection);
                get_role.Parameters.Add(role_id);
                get_role.Parameters["RoleId"].Value = rl_id;
                SqlDataReader role_reader = get_role.ExecuteReader();

                while (role_reader.Read())
                {
                    account.Role = role_reader["Name"].ToString();
                }

                if (role_reader != null && !role_reader.IsClosed)
                    role_reader.Close();
            }

            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();

            return account;
        }

        private string GenerateJWT(Account user)
        {
            var authParams = authOptions.Value;

            var securitykey = authParams.GetSymmetricSecurityKey();
            var creditials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim("Email", user.Mail),
                new Claim("Sub", user.Id.ToString()),
                new Claim("role", user.Role)
            };

            var token = new JwtSecurityToken(
                authParams.Issuer,
                authParams.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(authParams.TokenLifetime),
                signingCredentials: creditials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
