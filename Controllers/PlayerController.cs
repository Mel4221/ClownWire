using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO.Compression;
using QuickTools.QCore;
using QuickTools.QIO;   

namespace ClownWire.Controllers
{
    [Route("clownwire/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        [HttpGet()]
        public IActionResult Get()
        {
            /*
            string html, ip;
            html = System.IO.File.ReadAllText("wwwroot/player4.html");
            ip = Tools.GetLocalIPAddress();
            html = html.Replace("@ipaddress", $"http://{ip}");
            byte[] buffer = Encoding.ASCII.GetBytes(html);
            return File(buffer, "text/html");
            */
            return Ok("Pending to be completed...");
        }
    }
}