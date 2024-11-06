using System;
using System.Net;

namespace UShare.Controllers
{

class Tools
{
    public static string StoragePath = Path.Combine(Directory.GetCurrentDirectory(), "Cloud");
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
}

}