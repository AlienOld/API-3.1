using API_3._1.Resource.Api.Classes;
using API_3._1.Resource.Api.ConnectString;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_3._1.Resource.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupDbController : ControllerBase
    {
        // CreateBackUp: api/BackupDb/create
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateBackUp()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.str))
            {
                SqlCommand command;
                string destdir = @"C:\backupdb";
                if (!Directory.Exists(destdir))
                {
                    Directory.CreateDirectory(destdir);
                }
                try
                {
                    conn.Open();
                    command = new SqlCommand($@"backup database API_DB to disk='{destdir}\{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.Bak'", conn);
                    int execute = await command.ExecuteNonQueryAsync();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }


        // Get all BackUps: api/BackupDb/getall
        [HttpGet]
        [Route("getall")]
        public IEnumerable<string> getAllBackUps()
        {
            string destdir = @"C:\backupdb";
            string[] files = Directory.GetFiles(destdir);
            Backups backups = new Backups();
            foreach (string file in files)
            {
                var arr = file.Split(@"\");
                string name = arr[arr.Length - 1];
                backups.List.Add(name);
            }

            return backups.List;
        }


        // ReturnToBackUp: api/BackupDb/return
        [HttpPost]
        [Route("return")]
        public async Task<IActionResult> ReturnToBackUp([FromBody] JObject obj)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString.str))
            {
                SqlCommand command;
                string name = obj["name"].ToString();

                try
                {
                    conn.Open();
                    string destdir = @"C:\backupdb";
                    command = new SqlCommand($@"Restore database API_DB from disk='{destdir}\{name}' With Replace", conn);
                    int execute = await command.ExecuteNonQueryAsync();
                    return Ok("Database restored successfully");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
