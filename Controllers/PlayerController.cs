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
    [Route("clownwire/mp3")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        [HttpGet()]
        public IActionResult Get()
        {
            FilesMaper maper = new FilesMaper(ServerTools.CloudRootPath);
            maper.Map();
            string html, url;
            url = $"http://{ServerTools.GetLocalIPAddress()}/clownwire/src/cloud/";
            html = ServerTools.GetHtmlContent("you");
            StringBuilder songs = new StringBuilder();
            for(int f = 0; f < maper.Files.Count; f++)
            {
                string file = maper.Files[f];
                string fileId = new FileStream(file,FileMode.Open).Length.ToString();
                if(IGet.FileExention(file)=="mp3")
                {
                    ///<!--li class="Song" data-src="@path_file">@file_name</li-->
                    songs.Append($"<li class=\"Song\" data-src=\"{url}{fileId}\">{Path.GetFileNameWithoutExtension(file)}</li>");
                }
            }

            html = html.Replace("@list_of_songs",songs.ToString());
            byte[] buffer = Encoding.ASCII.GetBytes(html);
            return File(buffer, "text/html");

        }   
    }
}