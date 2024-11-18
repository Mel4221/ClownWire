using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickTools.QCore;
using QuickTools.QIO;
using System.IO;
using System.Text; 
using System.Threading.Tasks;

namespace ClownWire.Controllers
{

    //[Route("clownwire")]
    [Route("/")]
    [Route("/clownwire")]
    [ApiController]
    public class clownwireController : Controller
    {
        private static string _storagePath = ServerTools.CloudRootPath;

        [HttpGet]
        public IActionResult Get()
        {
            if(!ServerTools.Exists(this.ControllerContext))return NotFound();
            
            byte[] buffer = ServerTools.GetHtmlBuffer(this.ControllerContext);

            string mimeType = ServerTools.GetMimeType(this.ControllerContext);
              
            return File(buffer,mimeType);

        }
[HttpGet("{fileName}")]
public IActionResult GetWebFile(string fileName)
{
    // Get the file content and MIME type
    string file = ServerTools.GetFilePath(fileName);
    IGet.Yellow($"GET: {file}");
    if (!ServerTools.Exists(file)) return NotFound($"File not found: {file}");

    string mimeType = ServerTools.GetMimeType(file);
    
    // Ensure the file stream remains open for the duration of the file transfer
    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
    
    return File(fileStream, mimeType, Path.GetFileName(file));
}


    }
}