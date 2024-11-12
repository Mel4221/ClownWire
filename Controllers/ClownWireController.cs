using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text; 
using System.Threading.Tasks;

namespace ClownWire.Controllers
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
            [HttpGet("{fileName}")]
     public IActionResult GetFileName(string fileName)
{
    // Get the file content and MIME type
    string file = Tools.GetFile(fileName);
    if(!System.IO.File.Exists(file))return NotFound($"{fileName}");
    string html = System.IO.File.ReadAllText(file);
    string ip = Tools.GetLocalIPAddress();
    string mimeType = Tools.GetMimeType(fileName);

    // Check if the MIME type indicates binary data (e.g., image, video, PDF, etc.)
    if (Tools.IsBinaryMimeType(mimeType))
    {
        // For binary files, just return the file as is (without modifying the content)
        byte[] fileBytes = System.IO.File.ReadAllBytes(Tools.GetFile(fileName));
        return File(fileBytes, mimeType);
    }
    else
    {
        // For text-based files (HTML, JS, CSS, etc.), replace the placeholder
        html = html.Replace("@ipaddress", $"http://{ip}");

        // Convert the modified HTML to a byte array and return
        byte[] buffer = Encoding.ASCII.GetBytes(html);
        return File(buffer, mimeType);
    }
}

    }
}