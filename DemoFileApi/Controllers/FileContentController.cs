using DemoFileApi.Models;
using DemoFileApi.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace DemoFileApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileContentController : ControllerBase
    {
        private readonly DbConnection _connection;

        public FileContentController(DbConnection connection)
        {
            _connection = connection;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_connection.ExecuteReader("AppUser.CSP_GetFiles", true, dr => dr.ToFileDescription()));
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(_connection.ExecuteReader("AppUser.CSP_GetFiles", true, dr => dr.ToFileDescription(), new { All = true }));
        }

        [HttpGet("{uid}")]
        public IActionResult Get(Guid uid)
        {
            FileContent? fileContent = _connection.ExecuteReader("AppUser.CSP_GetFileById", true, dr => dr.ToFileContent(), new { Uid = uid }).SingleOrDefault();
            if(fileContent is null)
            {
                return NotFound();
            }

            return Ok(fileContent);
        }

        [HttpDelete("{uid}")]
        public IActionResult Delete(Guid uid) 
        {
            int rows = _connection.ExecuteNonQuery("AppUser.CSP_DeleteFile", true, new { Uid = uid });

            if(rows == 1)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
