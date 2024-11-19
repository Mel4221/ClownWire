using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickTools.QCore;
using QuickTools.QIO;
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
    
    [HttpGet("cloud/{fileName}")]
    public IActionResult GetCloud(string fileName)
    {

      FilesMaper maper = new FilesMaper(ServerTools.CloudRootPath);
      FileStream fileStream;
      string file,fileId; 
      maper.Map(); 
      for(int item = 0; item < maper.Files.Count; item++)
      {
           
          file = maper.Files[item]; 
          fileStream = new FileStream(file,FileMode.Open,FileAccess.Read);
          fileId = fileStream.Length.ToString();
          if(fileId == fileName)
          {
            return File(fileStream,ServerTools.GetMimeType(file));
          }
      }
      return NotFound(fileName);

    }
    [HttpGet("{fileName}")]
     public IActionResult GetSrc(string fileName)
{
       string file = ServerTools.GetFilePath(fileName);
       //IGet.Red($"FILE: {file}");
       string ext = IGet.FileExention(file);
         //     IGet.Red($"EXT: {ext}");

    if(!ServerTools.Exists(file))return NotFound($"{file}");
    // Get the file content and MIME type
    string mimeType = ServerTools.GetMimeType(file);
    //IGet.Red($"MIME_TYPE: {mimeType}");
    
    // Check if the MIME type indicates binary data (e.g., image, video, PDF, etc.)
    if (ext == "js"||ext=="html"||ext=="css")
    {
      // Convert the modified HTML to a byte array and return
          
        byte[] buffer =  ServerTools.GetContentBuffer(file);
        return File(buffer, mimeType);
    }
    else
    {
                // For binary files, just return the file as is (without modifying the content)
        byte[] fileBytes = System.IO.File.ReadAllBytes(file);
        return File(fileBytes, mimeType);
      
    }
}




    }
}