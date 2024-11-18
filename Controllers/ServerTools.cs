using System;
using System.Net;
using System.IO; 
using QuickTools.QCore;
using QuickTools.QIO;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ClownWire.Controllers
{

partial class ServerTools
{

    public static string CloudRootPath = Path.Combine(Directory.GetCurrentDirectory(), "Cloud");
    public static bool IsMediaFile(string fileName)
    {
              // List of common video file extensions supported by VLC
        List<string> supportedExtensions = new List<string>
        {
            ".mp4",   // MPEG-4 Part 14
            ".avi",   // Audio Video Interleave
            ".mkv",   // Matroska Video
            ".mov",   // QuickTime Movie
            ".wmv",   // Windows Media Video
            ".flv",   // Flash Video
            ".webm",  // WebM Video
            ".mpg",   // MPEG Video
            ".mpeg",  // MPEG Video
            ".ogv",   // Ogg Video
            ".3gp",   // 3GPP Video
            ".asf",   // Advanced Streaming Format
            ".rm",    // RealMedia Video
            ".rmvb",  // RealMedia Variable Bitrate Video
            ".ts",    // MPEG Transport Stream
            ".iso",   // ISO Disc Image (may contain video files)
            ".divx",  // DivX Video
            ".xvid",  // Xvid Video
            ".vob",   // Video Object (DVD)
            ".mts",   // AVCHD Video
            ".m2ts",  // AVCHD Video
            ".bik",   // Bink Video
            ".yuv",   // YUV Video Format
            ".h264",  // H.264 video format (container or codec)
            ".hevc",  // High-Efficiency Video Coding (H.265)
            ".vp8",   // VP8 Video (used in WebM)
            ".vp9"    // VP9 Video (used in WebM)
        };

        // Get the file extension and convert it to lowercase to make the comparison case-insensitive
        string fileExtension = Path.GetExtension(fileName)?.ToLower();

        // Return true if the extension is in the supported list, otherwise false
        return supportedExtensions.Contains(fileExtension);
    
    }
    public static string CleanFileName(string fileName)
    {
        
       // Step 1: Replace dots (.) with spaces unless they are part of file extensions (e.g., .mkv, .mp4, etc.)
        // Step 2: Remove unwanted characters like hyphens, underscores, and brackets
        // Step 3: Ensure there is a space between episode information and seasons if needed.

        // Remove brackets and hyphens first (we'll handle dots later)
        string cleanedName = Regex.Replace(fileName, @"[\[\]-]", " ");

        // Replace multiple spaces with a single space
        cleanedName = Regex.Replace(cleanedName, @"\s+", " ").Trim();

        // Step 1: Replace dots with spaces (except for file extension like .mp4, .avi, etc.)
        cleanedName = Regex.Replace(cleanedName, @"\.(?=[a-zA-Z0-9])", " "); // Replace dots that are followed by alphanumeric characters

        // Step 2: Normalize spaces and capitalize words to make it more human-readable
        cleanedName = Regex.Replace(cleanedName, @"\s+", " ").Trim();

        // Capitalize the first letter of each word (optional, but makes it more readable)
        var words = cleanedName.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }
        cleanedName = string.Join(" ", words);

        return cleanedName;

          
    }

    public static bool Exists(ControllerContext context)
    {
        string path = GetFilePath(context); 
        return System.IO.File.Exists(path);
    }
    public static bool Exists(string file)
    {
        return System.IO.File.Exists(file);
    }


    public static byte[] GetHtmlBuffer(ControllerContext context)
    {
        string controller = GetController(context);
        return  Encoding.ASCII.GetBytes(GetHtmlContent(controller));
        //this.ControllerContext.ActionDescriptor.ControllerName.ToLower();
    }
     public static byte[] GetHtmlBuffer(string html)
    {
        return  Encoding.ASCII.GetBytes(GetHtmlContent(html));
        //this.ControllerContext.ActionDescriptor.ControllerName.ToLower();
    }
    public static string GetController(ControllerContext context)=> context.ActionDescriptor.ControllerName.ToLower();
    public static string GetHtmlContent(ControllerContext context)
    {
        return GetHtmlContent(GetController(context)); 
        //this.ControllerContext.ActionDescriptor.ControllerName.ToLower();
    }


    public static byte[] GetContentBuffer(string file)
    {
       return IGet.Bytes(GetContent(file));
    }
    public static string GetContent(string file)
    {
    
        string ip,content;
        //get ip address 
        ip = GetLocalIPAddress();
        //swap file path with its content 
        content = System.IO.File.ReadAllText(file);
        //replace any reference to the ip address
        content = content.Replace("@ip",$"http://{ip}"); 
       return content;
    }
    public static string GetHtmlContent(string file)
    {
        string ip,content;
        //get ip address 
        ip = GetLocalIPAddress();
        //gets the file path 
        file = GetHtml(file);
        //swap file path with its content 
        content = System.IO.File.ReadAllText(file);
        //replace any reference to the ip address
        content = content.Replace("@ip",$"http://{ip}"); 
       return content;
    }
    public static string GetFilePath(ControllerContext context)
    {
        string controller_name = context.ActionDescriptor.ControllerName.ToLower();
        return GetFilePath(controller_name);
    }

   
    public static string GetFilePath(string file)
    {
        FilesMaper maper = new FilesMaper(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
        //maper.AllowDebugger = true; 
        maper.Map();
        string path = "";
        
        foreach(string item in maper.Files)
        {
            path = Path.GetFileName(item);

            if(file == path)
            {
               // Get.Green($"{item} OK");
                return item;
            }
        }
        return string.Empty; 
    }

    public static string GetHtml(string file)
    {
        FilesMaper maper = new FilesMaper(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
        //maper.AllowDebugger = true; 
        maper.Map();
        string path = "";
        
        foreach(string item in maper.Files)
        {
            if(Get.FileExention(item) == "html")
            {
                path = Path.GetFileNameWithoutExtension(item);

                if(file == path)
                {
                // Get.Green($"{item} OK");
                    return item;
                }
            }
         
        }
        return string.Empty; 
    }

public static string FormatFileSize(long sizeInBytes)
{
    if (sizeInBytes >= 1024 * 1024)
        return $"{sizeInBytes / (1024 * 1024)} MB";
    else if (sizeInBytes >= 1024)
        return $"{sizeInBytes / 1024} KB";
    else
        return $"{sizeInBytes} Bytes";
}


    public static string GetLocalIPAddress()
    {  string ipAddress = "Not available";

        try
        {
            // Get all IP addresses associated with the machine.
            var host = Dns.GetHostEntry(Dns.GetHostName());

            // Find the first valid IPv4 address (skip loopback address).
            ipAddress = host.AddressList
                            .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !ip.ToString().StartsWith("127"))
                            ?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving local IP address: " + ex.Message);
        }

        return ipAddress ?? "Not available";
    }

      private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {".html", "text/html"},
        {".htm", "text/html"},
        {".css", "text/css"},
        {".js", "application/javascript"},
        {".ts", "application/typescript"},
        {".json", "application/json"},
        {".xml", "application/xml"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".png", "image/png"},
        {".gif", "image/gif"},
        {".svg", "image/svg+xml"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mkv", "video/mkv"},
        {".wav", "audio/wav"},
        {".pdf", "application/pdf"},
        {".zip", "application/zip"},
        {".txt", "text/plain"},
        {".md", "text/markdown"},
        {".ico", "image/vnd.microsoft.icon"},
        {".webp", "image/webp"},
        // Add more MIME types as needed
    };

     public static string GetMimeType(ControllerContext context)
    {
        return GetMimeType(GetController(context)+".html");
    }
    public static string GetMimeType(string file)
    {

        string fileName = file; 
        if(!file.Contains('.')){
            fileName = file+".html";
        }

        // Get the file extension (ensure it's lowercase for comparison)
        var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

        // Check if the extension exists in our dictionary
        if (MimeTypes.TryGetValue(fileExtension, out string mimeType))
        {
            return mimeType;
        }

        // If the file type is unknown, return a default type
        return "application/octet-stream";  // Default MIME type for binary files
    }



// Helper method to determine if the MIME type is binary
public static bool IsBinaryMimeType(string mimeType)
{
    // List of binary MIME types (you can extend this list as needed)
    var binaryMimeTypes = new List<string>
    {
        "image/", // image types
        "audio/", // audio types
        "video/", // video types
        "application/pdf", // PDF files
        "application/octet-stream", // generic binary stream
        "application/zip", // ZIP files
        "application/msword", // Word documents
        "application/vnd.ms-excel", // Excel files
        "application/vnd.ms-powerpoint",
        "application/" // PowerPoint files
    };

    // Check if the MIME type starts with a binary type or is exactly one of the known binary MIME types
    return binaryMimeTypes.Any(mimeType.StartsWith);
}



   public static ProcessStartInfo StartInfo(string arguments)
                {
                        return new ProcessStartInfo()
                        {
                                FileName="ffmpeg",
                                Arguments=$"-i {arguments}",
                                RedirectStandardError = true,
                                UseShellExecute = false, // Required for redirection
                                CreateNoWindow = true // Optional: do not create a window
                        };
                }
// Method to get the duration in HH:MM:SS format
    public async static Task<string> GetDurationStamp(string fileName)
    {
        
            Process process = Process.Start(StartInfo(fileName));
            //wait that the whole process is completed
            string output = process.StandardError.ReadToEnd();


            process.WaitForExit();
            
    
               // Console.WriteLine($"OUTPUT: \n{output}");
                string[] words = output.Split(' ');
                StringBuilder text = new StringBuilder();
                for(int word = 0; word < words.Length; word++)
                {

                        text.Append($"[{words[word]}]\n");
                        if(words[word] == "DURATION")
                        {
                            string duration = words[word+9];
                              TimeSpan stamp;
                                if (TimeSpan.TryParse(duration, out stamp))
                                {
                                    // Format it to "hh:mm:ss" (ignoring fractional seconds)
                                    return stamp.ToString(@"hh\:mm\:ss");
                                }
                        }   
                }
               
        return "00:00:00"; // "00:00:00"+Default if the duration could not be parsed
    }

    // Method to get the total duration in minutes as a double (decimal)
    public async static Task<double> GetDurationMinutes(string fileName)
    {
            string durationString = await GetDurationStamp(fileName);
            
            // Split the result string by the ':' character to get an array of time parts
            string[] parts = durationString.Split(':');

                if (parts.Length == 3)
                {
                    // Convert to total minutes (HH * 60 + MM + SS/60)
                    double hours = Convert.ToDouble(parts[0]);
                    double minutes = Convert.ToDouble(parts[1]);
                    double seconds = Convert.ToDouble(parts[2]);

                    return Math.Floor(hours * 60 + minutes + (seconds / 60));
                }
            
        
            return 0; // Default if the duration could not be parsed
    }

        public static string ToStamp(int totalMinutes)
    {
        // Calculate hours, minutes, and remaining seconds
        int hours = totalMinutes / 60;  // 1 hour = 60 minutes
        int minutes = totalMinutes % 60;  // Remainder minutes after calculating hours
        int seconds = 0;  // We assume it's in minutes, so seconds are always 00

        // Return the formatted string as "hh:mm:ss" with zero-padded values
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

}

}