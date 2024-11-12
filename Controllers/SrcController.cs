using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickTools.QCore;
using System.IO;
using System.Text; 
using System.Threading.Tasks;

namespace ClownWire.Controllers
{
    [Route("clownwire/[controller]")]
    [Route("[controller]")]
    [ApiController]    
    public class SrcController: ControllerBase
    {
        [HttpGet("{fileName}")]
     public IActionResult GetSrc(string fileName)
{
       string file = Tools.GetFile(fileName);
    if(!System.IO.File.Exists(file))return NotFound($"{fileName}");
    // Get the file content and MIME type
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