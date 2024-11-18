using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ClownWire.Controllers
{
    [Route("clownwire/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            return Ok("This endpoint is not in use currently");
        }
       /*
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
           string result= await Tools.GetDurationStamp(filePath);
           double minutes = await Tools.GetDurationMinutes(filePath);
           return Ok($"Stamp: {result} Minutes: {minutes}"); 
        }
        */
/*
        // Web API method to stream video file with CORS headers and partial content support
        [HttpGet("stream")]
        public async Task<IActionResult> Get()
        {
            string rootPath,mkv_file,webm_file,minute;
            rootPath = "wwwroot/src/media/";
            mkv_file = Path.Combine(rootPath,"video.mkv");
            minute = Request.Headers["Minute"].ToString();
            webm_file = Path.Combine(rootPath,$"{minute}.webm");

            //Response.Headers.Add("Test",$"{minute}");
            //Response.Headers.Add("Test",$"from: {Tools.ToStamp(1)} to: {Tools.ToStamp(2)}");
            //Response.Headers.Add("Test",$"from: {Tools.ToStamp(int.Parse(minute))} to: {Tools.ToStamp(int.Parse(minute)+1)}");
            if(!System.IO.File.Exists(webm_file))
            {
                await ProcessMKVAsync(mkv_file,webm_file,Tools.ToStamp(int.Parse(minute)),Tools.ToStamp(int.Parse(minute)+1));
            }
            //webm_file = Path.Combine(rootPath,"video");
            // Set the CORS headers globally for the response
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
           // Response.Headers.Add("Access-Control-Allow-Headers", "Range");

            // Check if Range header is present
           // var rangeHeader = Request.Headers["Range"].ToString();
            /*
            if (string.IsNullOrEmpty(rangeHeader))
            {
                return BadRequest("Range header is required.");
            }
            */

            // Parse the Range header (e.g., "bytes=0-1023")
            //long startByte, endByte;
            /*
            if (!TryParseRangeHeader(rangeHeader, out startByte, out endByte))
            {
                return BadRequest("Invalid Range header.");
            }
            

            // Ensure the file exists
            var mkvFile = new FileInfo(mkv_file);
            if (!mkvFile.Exists)
            {
                return NotFound("Media file not found.");
            }

            var totalFileSize = mkvFile.Length;
            var totalMinutes = await Tools.GetDurationMinutes(mkv_file);
            /*
            // Validate the range
            if (startByte > totalFileSize)
            {
                return BadRequest("Start byte is beyond the end of the file.");
            }
            
            // Set the range end to the file length if it exceeds the file size
           // endByte = (endByte >= totalFileSize) ? totalFileSize - 1 : endByte;

            // Set the response status code to Partial Content
            Response.StatusCode = 200;
            Response.ContentType = "video/webm";  // This sets the content type to video/webm

            // Set the range header without specifying the content length
            //Response.Headers.Add("Content-Range", $"bytes {startByte}-{endByte}/{totalFileSize}");
            //Response.Headers.Add("Content-Range", $"bytes {startByte}-{endByte}/{totalFileSize}");

            Response.Headers.Add("Total-Minutes",$"{totalMinutes}");
            // Open the file and stream the requested chunk
            using (var fileStream = new FileStream(webm_file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                //fileStream.Seek(startByte, SeekOrigin.Begin);
                var buffer = new byte[int.Parse(fileStream.Length.ToString())];
                int bytesRead;
               // Console.WriteLine($"Start Byte: [{startByte}]");

                // Stream data using chunked transfer encoding
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await Response.Body.WriteAsync(buffer, 0, bytesRead);
                }
            }

            return new EmptyResult(); // End the response
        }

        /*
        // Method to parse the Range header (e.g., "bytes=0-1023")
        private bool TryParseRangeHeader(string rangeHeader, out long startByte, out long endByte)
        {
            // Default values
            startByte = 0;
            endByte = 0;

            if (rangeHeader.StartsWith("bytes="))
            {
                var rangeParts = rangeHeader.Substring(6).Split('-');
                if (rangeParts.Length == 2)
                {
                    // Parse both start and end
                    if (string.IsNullOrEmpty(rangeParts[1]))
                    {
                        // Range is specified as "bytes=start-" (only start provided)
                        if (long.TryParse(rangeParts[0], out startByte))
                        {
                            endByte = long.MaxValue; // Read till the end of the file
                            return true;
                        }
                    }
                    else
                    {
                        // Range is specified as "bytes=start-end"
                        if (long.TryParse(rangeParts[0], out startByte) && long.TryParse(rangeParts[1], out endByte))
                        {
                            return true;
                        }
                    }
                }
            }

            return false; // Invalid range
        }

*/

        /*
        // Method to process the video file asynchronously using ffmpeg
        private async Task ProcessMKVAsync(string fileIn,string fileOut,string from , string to)
        {
            // Process the video asynchronously using ffmpeg
            await Task.Run(() =>
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i {fileIn} -ss {from} -to {to} -c:v vp8 -c:a libvorbis -g 60 -keyint_min 60 -map 0:v:0 -map 0:a:0 -f webm {fileOut}"
                    //Arguments = $"-i {fileIn} -ss {from} -to {to} -map 0:v:0 -map 0:a:0 -c:v vp8 -c:a libvorbis -f webm {fileOut}",
                    //Arguments = $"ffmpeg -i {fileIn} -ss {from} -to {to} -map 0:v:0 -map 0:a:0 -c:v vp8 -c:a libvorbis -f webm {fileOut}"
                    //RedirectStandardOutput = true
                    //RedirectStandardError = true,
                    //UseShellExecute = false
                    //Arguments = $"-i {fileIn} -ss {from} -to {to} -c:v vp8 -c:a libvorbis -map 0:v:0 -map 0:a:0 -f webm {fileOut}"

                };

                using (var process = Process.Start(processStartInfo))
                {
                    process.WaitForExit();
                }
            });
        }
        */
    }
}
