using ClownWire.Controllers;
using Microsoft.AspNetCore.Mvc;
using QuickTools.QCore;
using QuickTools.QIO;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileStreamingApi.Controllers
{

    [Route("clownwire/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {    
            string mimeType = ServerTools.GetMimeType("stream");
            
            byte[] buffer = ServerTools.GetHtmlBuffer("stream");
            // Return the index.html file and set the content type as "text/html"
            // return File("index.html", "text/html");
            return File(buffer, mimeType);
        }
 
  [HttpGet("links")]
public IActionResult GetLinks()
{
    // Initialize the FilesMaper to get the files from the specified path
    FilesMaper maper = new FilesMaper(ServerTools.CloudRootPath);
    maper.Map();

    // Initialize StringBuilder to construct a JSON response
    StringBuilder jsonResponse = new StringBuilder();
    
    // Start the JSON array
    jsonResponse.Append("[");

    // Loop through the list of files and add them to the JSON string
    for (int i = 0; i < maper.Files.Count; i++)
    {
        string file = Path.GetFileNameWithoutExtension(maper.Files[i]);
        string fileName = ServerTools.CleanFileName(file);
        string fileId = fileName; 
        if(ServerTools.IsMediaFile(maper.Files[i]))
        {
            // Append file details in JSON format
            jsonResponse.Append("{");
            jsonResponse.Append($"\"fileName\": \"{fileName}\", ");
            jsonResponse.Append($"\"fileId\": \"{fileId}\"");
       
            // If it's not the last file, add a comma
            if (i < maper.Files.Count - 1)
            {
                jsonResponse.Append("}, ");
            }
            else
            {
                jsonResponse.Append("}");
            }
        }
    }
    // End the JSON array
    jsonResponse.Append("]");

    // Return the JSON string as the response
    return Content(jsonResponse.ToString(), "application/json");
}



        [HttpGet("{fileId}")]
        public async Task<IActionResult> StreamFile(string fileId)
        {
            // Path to your media file (adjust this to the correct path on your server)
            FilesMaper maper = new FilesMaper(ServerTools.CloudRootPath);
            maper.Map();

            string filePath ="";
            foreach(string file in maper.Files)
            {
                string id = ServerTools.CleanFileName(Path.GetFileNameWithoutExtension(file));
                //string length = new FileStream(file,FileMode.Open).Length.ToString();
                if(fileId == id){
                    filePath = file; 
                    break;
                }
            }

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileInfo = new FileInfo(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Get the Range header if present
            var rangeHeader = Request.Headers["Range"].ToString();
            if (!string.IsNullOrEmpty(rangeHeader))
            {
                // Parse the range header: "bytes=0-999"
                var range = rangeHeader.Replace("bytes=", "").Split("-");
                long start = long.Parse(range[0]);
                long end = range.Length > 1 && !string.IsNullOrEmpty(range[1]) ? long.Parse(range[1]) : fileInfo.Length - 1;

                // Validate the range (start byte must be less than end byte, and start must be within the file)
                if (start >= fileInfo.Length || start > end)
                {
                    return BadRequest("Invalid range.");
                }

                // Seek to the start position in the file
                fileStream.Seek(start, SeekOrigin.Begin);

                // Calculate the content length (number of bytes to return)
                var contentLength = end - start + 1;

                // Set the response status code to "206 Partial Content"
                Response.StatusCode = 206;

                // Add Content-Range header to the response
                Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileInfo.Length}");

                // Return the file stream with the specified range
                return new FileStreamResult(fileStream, ServerTools.GetMimeType(filePath))
                {
                    EnableRangeProcessing = true,  // This enables range processing for the client (e.g., VLC)
                    FileDownloadName = fileInfo.Name
                };
            }

            // If no range is requested, return the whole file
            Response.Headers.Add("Accept-Ranges", "bytes");
            return new FileStreamResult(fileStream,  ServerTools.GetMimeType(filePath))
            {
                FileDownloadName = fileInfo.Name,
            };
        }
    }
}
