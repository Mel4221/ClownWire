using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text; 
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;


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
            
        //private readonly ILogger<FileUploadController> _logger;
        /*
        public FileUploadController(ILogger<FileUploadController> logger)
        {
            _logger = logger;
        }
        */

        [HttpPost("astree")]
        public IActionResult UploadAsTree()
        {
            return Ok();
        }

/*
        // The endpoint to handle the file tree upload
        [HttpPost("astree")]
        public async Task<IActionResult> UploadAsTree()
        {
            try
            {
                // Ensure the incoming request contains the correct multipart form-data content type
                if (!Request.HasFormContentType)
                {
                    return BadRequest("Expected form data.");
                }

                var formData = await Request.ReadFormAsync();

                // Retrieve the file tree JSON from the form data
                var fileTreeJson = formData["fileTree"];
                var text = formData["text"]; // Retrieve any associated text (optional)

                if (string.IsNullOrEmpty(fileTreeJson))
                {
                    return BadRequest("File tree is missing.");
                }

                // Deserialize the file tree JSON into a C# object (e.g., Dictionary or custom class)
                var fileTree = JsonSerializer.Deserialize<Dictionary<string, object>>(fileTreeJson);

                // Process the file tree (you can store it in a database, for example)
                // Here you can add your logic to handle the file tree data
                // Example: Save the tree to disk, save metadata, etc.

                // If handling files in chunks, look for the 'fileChunk' part of the request
                var fileChunks = formData.Files;
                if (fileChunks.Count > 0)
                {
                    foreach (var chunk in fileChunks)
                    {
                        // You can save chunks to a temporary folder before completing the upload
                        var filePath = Path.Combine("TempUploads", chunk.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await chunk.CopyToAsync(stream);
                        }

                        // You can track chunk index to associate it with the correct file
                        // Example: Save chunk info and associate with the file tree
                    }
                }

                // Return a success response
                return Ok(new { message = "Upload successful", fileTree = fileTree });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file tree.");
                return StatusCode(500, "Internal server error.");
            }
        }
        */

[HttpPost("file")]
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
    var chunkFilePath = Path.Combine(_storagePath, $"{fileName}.part{chunkIndex}");
    /*
    if(!Directory.Exists(Path.GetDirectoryName(chunkFilePath)))
    {
        Directory.CreateDirectory(Path.GetDirectoryName(chunkFilePath));
    }
    */

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