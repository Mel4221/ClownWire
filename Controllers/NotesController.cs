using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text; 
using System.Threading.Tasks;

namespace ClownWire.Controllers
{
    [Route("clownwire/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private static string _storagePath = ServerTools.CloudRootPath;

        [HttpGet()]
        public IActionResult Download()
        {   
            /*
            string html,ip;
            html = System.IO.File.ReadAllText("wwwroot/notes.html");
            ip = Tools.GetLocalIPAddress();
            html = html.Replace("@ipaddress",$"http://{ip}"); 
            //Console.WriteLine(html);
            byte[] buffer = Encoding.ASCII.GetBytes(html);
            // Return the index.html file and set the content type as "text/html"
            // return File("index.html", "text/html");
            return File(buffer, "text/html");
            */
            return Ok("Pending not completed yet...");
        }

[HttpGet("filelist")]
public IActionResult GetFileList()
{
            return Ok("Pending not completed yet...");
    /*
    // Ensure the directory exists
    if (!Directory.Exists(_storagePath))
    {
        return NotFound("Directory not found.");
    }

    // Get all files from the storage directory
    var files = Directory.GetFiles(_storagePath);

    // Create a list to hold file details (name and size)
    var fileDetails = files.Select(file => new
    {
        FileName = Path.GetFileName(file),
        FileSize = Tools.FormatFileSize(new FileInfo(file).Length)// Size in bytes
    }).ToList();

    // Return the list of file details as a JSON response
    return Ok(fileDetails);
    */
}



       // GET api/file/download/{fileName}
       [HttpGet("{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
                        return Ok("Pending not completed yet...");

            /*
            var filePath = Path.Combine(_storagePath, fileName);
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Stream the file directly to the client
            return File(fileStream, "application/octet-stream", fileName);
            */
        }

    }
}
