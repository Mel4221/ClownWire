using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text; 
using System.Threading.Tasks;

using QuickTools.QCore;

namespace ClownWire.Controllers
{
    [Route("clownwire/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private static string _storagePath = ServerTools.CloudRootPath;
        [HttpGet()] 
        public IActionResult Get()
        {

            string mimeType = ServerTools.GetMimeType(this.ControllerContext);
            IGet.Box($"DEBUGG");
            byte[] buffer = ServerTools.GetHtmlBuffer(this.ControllerContext);
            // Return the index.html file and set the content type as "text/html"
            // return File("index.html", "text/html");
            return File(buffer, mimeType);

        }
            
[HttpPost("upload")]
public async Task<IActionResult> UploadFile([FromForm] string text)
{
    var request = HttpContext.Request;

    // Check if the request has a multipart content type
    if (!request.HasFormContentType)
    {
        return BadRequest("No form data submitted.");
    }

    var form = await request.ReadFormAsync();
    var fileChunk = form.Files.FirstOrDefault();

    if (fileChunk == null || fileChunk.Length == 0)
    {
        return BadRequest("No file uploaded.");
    }

    var chunkIndex = int.Parse(form["chunkIndex"]);
    var totalChunks = int.Parse(form["totalChunks"]);
    var fileName = fileChunk.FileName;


    // Save the chunk to disk with a temporary name
    var chunkFilePath = Path.Combine(_storagePath, $"{ServerTools.CleanDirectoryName(fileName)}.part{chunkIndex}");
    if(!Directory.Exists(Path.GetDirectoryName(chunkFilePath)))
    {
        Directory.CreateDirectory(Path.GetDirectoryName(chunkFilePath));
    }

    using (var stream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
    {
        await fileChunk.CopyToAsync(stream);
    }

    // Check if all chunks have been uploaded
    if (chunkIndex == totalChunks - 1)
    {
        // Combine all chunks into a single file
        var finalFilePath = Path.Combine(_storagePath, fileName);
        using (var finalStream = new FileStream(finalFilePath, FileMode.Create))
        {
            for (int i = 0; i < totalChunks; i++)
            {
                var tempFilePath = Path.Combine(_storagePath, $"{fileName}.part{i}");
                using (var tempStream = new FileStream(tempFilePath, FileMode.Open))
                {
                    await tempStream.CopyToAsync(finalStream);
                }

                // Optionally delete the chunk file after merging
                System.IO.File.Delete(tempFilePath);
            }
        }
    }

    // Log the uploaded text for reference
    var percentage = (chunkIndex + 1) / totalChunks * 100;

    System.Console.WriteLine($"Uploaded {fileName} {percentage}%");

return Content($"{{\"message\": \"Chunk {chunkIndex + 1} uploaded successfully.\"}}", "application/json");
}
        public UploadController()
        {
            // Ensure the directory exists
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }
    }
}