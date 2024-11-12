using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO.Compression;
using QuickTools.QCore;
using QuickTools.QIO;   

namespace ClownWire.Controllers
{
    [Route("clownwire/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private static string _storagePath = Tools.StoragePath;

        // Track status and result of tasks using dictionaries
        private static ConcurrentDictionary<string, string> TaskStatus = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> TaskResult = new ConcurrentDictionary<string, string>();


        
        [HttpGet()]
        public IActionResult Get()
        {
            string html, ip;
            html = System.IO.File.ReadAllText("wwwroot/download.html");
            ip = Tools.GetLocalIPAddress();
            html = html.Replace("@ipaddress", $"http://{ip}");
            byte[] buffer = Encoding.ASCII.GetBytes(html);
            return File(buffer, "text/html");
        }



       
        [HttpGet("isync")]
        public IActionResult ISync()
        {
            string html, ip;
            html = System.IO.File.ReadAllText("wwwroot/isync.html");
            ip = Tools.GetLocalIPAddress();
            html = html.Replace("@ipaddress", $"http://{ip}");
            byte[] buffer = Encoding.ASCII.GetBytes(html);
            return File(buffer, "text/html");
        }

        // New endpoint to start the zip creation process asynchronously
        [HttpGet("start-zip")]
        public IActionResult StartZip()
        {
            // Generate a unique task ID for this operation
            string taskId = Guid.NewGuid().ToString();

            // Track the status of this task
            TaskStatus[taskId] = "Started";

            // Start the async operation to create the zip file
            Task.Run(() => CreateZipFileAsync(taskId));

            // Return the task ID for the client to check status
            return Ok(new { TaskId = taskId });
        }

        // Async method to create the zip file
        private async Task CreateZipFileAsync(string taskId)
        {
            string zipFileName = "songs.zip";
            string zipFilePath = Path.Combine(_storagePath, zipFileName);

            try
            {
                // Update status to "In Progress"
                TaskStatus[taskId] = "In Progress";

                // Create a zip file asynchronously
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        var files = Directory.GetFiles(_storagePath);

                        foreach (var file in files)
                        {
                            string fileName = Path.GetFileName(file);

                            // Add each file to the zip archive
                            var zipEntry = archive.CreateEntry(fileName);

                            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                            using (var entryStream = zipEntry.Open())
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }

                    // Save the file to disk
                    System.IO.File.WriteAllBytes(zipFilePath, memoryStream.ToArray());

                    // Mark the task as completed
                    TaskStatus[taskId] = "Completed";
                    TaskResult[taskId] = zipFilePath;
                }
            }
            catch (Exception ex)
            {
                // If something fails, update the status to failed
                TaskStatus[taskId] = $"Failed: {ex.Message}";
            }
        }

        // Endpoint to get the status of the zip file creation process
        [HttpGet("zip-status/{taskId}")]
        public IActionResult GetZipStatus(string taskId)
        {
            if (TaskStatus.TryGetValue(taskId, out var status))
            {
                return Ok(new { Status = status });
            }

            return NotFound("Task not found.");
        }

        // Endpoint to download the zip file once it is ready
        [HttpGet("download-zip/{taskId}")]
        public IActionResult DownloadZip(string taskId)
        {
            if (TaskResult.TryGetValue(taskId, out var zipFilePath) && System.IO.File.Exists(zipFilePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
                return File(fileBytes, "application/zip", "songs.zip");
            }

            return NotFound("Zip file not found or task failed.");
        }

        [HttpGet("sync")]
        public IActionResult GetSync()
        {
            string html, ip, link, file, tag;
            html = System.IO.File.ReadAllText("wwwroot/sync.html");
            ip = Tools.GetLocalIPAddress();
            StringBuilder songs = new StringBuilder();
            if (!Directory.Exists(_storagePath))
            {
                return NotFound("Directory not found.");
            }

            var files = Directory.GetFiles(_storagePath);
            foreach (string f in files)
            {
                file = Path.GetFileName(f);
                link = $"http://{ip}/clownwire/download/{file}";
                tag = $"<a target=\"{link}\" href=\"{link}\" class=\"links\" download=\"{file}\">{file}</a>";
                songs.Append(tag);
            }

            html = html.Replace("@SongsList", songs.ToString());
            byte[] buffer = Encoding.ASCII.GetBytes(html);
            return File(buffer, "text/html");
        }

        [HttpGet("filelist")]
        public IActionResult GetFileList()
        {
            if (!Directory.Exists(_storagePath))
            {
                return NotFound("Directory not found.");
            }

            var files = Directory.GetFiles(_storagePath);

            var fileDetails = files.Select(file => new
            {
                FileName = Path.GetFileName(file),
                FileSize = Tools.FormatFileSize(new FileInfo(file).Length)
            }).ToList();

            return Ok(fileDetails);
        }

        // Download file by name
        [HttpGet("{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var filePath = Path.Combine(_storagePath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", fileName);
        }
    }
}
