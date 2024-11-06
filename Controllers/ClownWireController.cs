using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text; 
using System.Threading.Tasks;

namespace UShare.Controllers
{
    [Route("clownwire")]
    [ApiController]
    public class clownwireController : Controller
    {
        private static string _storagePath = Tools.StoragePath;

        [HttpGet()]
        public IActionResult Get()
        {
            string html,ip;
            html = System.IO.File.ReadAllText("wwwroot/clownwire.html");
            ip = Tools.GetLocalIPAddress();
            html = html.Replace("@ipaddress",$"http://{ip}"); 
            //Console.WriteLine(html);
            byte[] buffer = Encoding.ASCII.GetBytes(html);
            // Return the index.html file and set the content type as "text/html"
            // return File("index.html", "text/html");
            return File(buffer, "text/html");

        }

    }
}