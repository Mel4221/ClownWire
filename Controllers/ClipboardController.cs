using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using QuickTools.QData.Bastard;

namespace ClownWire.Controllers
{
    [Route("clownwire/[controller]")]
    [ApiController]
    public class ClipboardController : ControllerBase
    {
        private static List<string> clipboardItems = new List<string>();

        // First HTTP GET - keeps your existing logic
        [HttpGet()]
        public IActionResult Get()
        {
            string mimeType = ServerTools.GetMimeType(this.ControllerContext);
            byte[] buffer = ServerTools.GetHtmlBuffer(this.ControllerContext);
            return File(buffer, mimeType);
        }

        // Get all clipboard items (GET /clownwire/clipboard/GetNotes)
        [HttpGet("GetNotes")]
        public IActionResult GetNotes()
        {
            try
            {
                if (Directory.Exists(ServerTools.CloudRootPath))
                {
                    Directory.CreateDirectory(ServerTools.CloudRootPath);
                }

                string db = Path.Combine(ServerTools.CloudRootPath, "Clipboard.db");
                ClownManager manager = new ClownManager(db);
                manager.Load();
                clipboardItems.Clear(); // Clear existing items to reload them
                for (int item = 0; item < manager.Keys.Count; item++)
                {
                    clipboardItems.Add(manager.Keys[item].Value);
                }

                // Return the list of clips as a JSON response
                return Ok(new { clips = clipboardItems });
            }
            catch (System.Exception ex)
            {
                // Log the exception if necessary
                return StatusCode(500, $"Error fetching clipboard items: {ex.Message}");
            }
        }

        // Save a new clipboard item (POST /clownwire/clipboard/Save)
        [HttpPost("Save")]
        public IActionResult Save()
        {
            // Get the clip text from the HTTP header "clip"
            string newClip = Request.Headers["clip"].ToString();

            if (string.IsNullOrWhiteSpace(newClip))
            {
                return BadRequest("Clip text is required.");
            }

            try
            {
                if (Directory.Exists(ServerTools.CloudRootPath))
                {
                    Directory.CreateDirectory(ServerTools.CloudRootPath);
                }

                string db = Path.Combine(ServerTools.CloudRootPath, "Clipboard.db");
                ClownManager manager = new ClownManager(db);
                manager.Create();
                manager.Load();
                manager.Add(new ClownKey
                {
                    Name = "clip",
                    Value = newClip
                });
                manager.Save();

                return Ok(new { success = true });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, $"Error saving the clip: {ex.Message}");
            }
        }
    }
}
