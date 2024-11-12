using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace FileStreamingApi.Controllers
{
    [Route("clownwire/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        [HttpGet()]
        public async Task<IActionResult> StreamFile()
        {
            // Path to your media file (adjust this to the correct path on your server)
            string filePath = "wwwroot/src/media/video.mkv";

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
                return new FileStreamResult(fileStream, "video/mkv")
                {
                    EnableRangeProcessing = true,  // This enables range processing for the client (e.g., VLC)
                    FileDownloadName = fileInfo.Name
                };
            }

            // If no range is requested, return the whole file
            Response.Headers.Add("Accept-Ranges", "bytes");
            return new FileStreamResult(fileStream, "video/mkv")
            {
                FileDownloadName = fileInfo.Name,
            };
        }
    }
}
