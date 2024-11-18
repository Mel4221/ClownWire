using System;
using System.IO;
using System.Text.RegularExpressions;
namespace ClownWire.Controllers
{
    partial class ServerTools
    {


    // This method cleans a file or directory name according to your specifications
    public static string CleanDirectoryName(string fileName)
    {
        // Step 1: Remove unwanted characters like brackets and hyphens
        string cleanedName = Regex.Replace(fileName, @"[\[\]-]", " ");

        // Step 2: Replace multiple spaces with a single space and trim any leading/trailing spaces
        cleanedName = Regex.Replace(cleanedName, @"\s+", " ").Trim();

        // Step 3: Handle dots that are not part of file extensions
        // First, we separate the file extension from the rest of the name
        string extension = Path.GetExtension(cleanedName);
        string nameWithoutExtension = cleanedName.Substring(0, cleanedName.Length - extension.Length);

        // Replace dots with spaces in the name part, excluding the file extension
        nameWithoutExtension = Regex.Replace(nameWithoutExtension, @"\.(?=[a-zA-Z0-9])", " ");

        // Step 4: Combine cleaned name and extension back together
        cleanedName = nameWithoutExtension + extension;

        // Step 5: Normalize spaces and capitalize words to make it more human-readable
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

    // This method ensures that all directories are created based on the cleaned file path
    public static void CreateDirectoryStructure(string rootPath, string[] subPaths)
    {
        string currentPath = rootPath;

        foreach (var subPath in subPaths)
        {
            // Clean the directory name
            string sanitizedSubPath = CleanDirectoryName(subPath);

            // Combine the cleaned subPath with the current directory path
            currentPath = Path.Combine(currentPath, sanitizedSubPath);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
                Console.WriteLine($"Directory created: {currentPath}");
            }
            else
            {
                Console.WriteLine($"Directory already exists: {currentPath}");
            }
        }
    }
 public static void CreateDirectoriesFromFile(string filePath)
    {
        // Check if the path is a directory
        if (Directory.Exists(filePath))
        {
            Console.WriteLine($"The provided path is a directory: {filePath}");

            // Loop through the files and directories recursively
            LoopDirectories(filePath);
        }
        else if (File.Exists(filePath))
        {
            Console.WriteLine($"The provided path is a file: {filePath}");

            // If it's a file, get the parent directory and ensure it exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Console.WriteLine($"Directory created: {directory}");
            }
        }
        else
        {
            Console.WriteLine($"The provided path is neither a valid file nor directory: {filePath}");
        }
    }

    private static void LoopDirectories(string directoryPath)
    {
        try
        {
            // Get all subdirectories in the current directory
            string[] subdirectories = Directory.GetDirectories(directoryPath);

            // Iterate through each subdirectory
            foreach (var subdir in subdirectories)
            {
                Console.WriteLine($"Found subdirectory: {subdir}");

                // Create the subdirectory if it doesn't exist
                if (!Directory.Exists(subdir))
                {
                    Directory.CreateDirectory(subdir);
                    Console.WriteLine($"Created directory: {subdir}");
                }

                // Recursively process the nested directories
                LoopDirectories(subdir);
            }

            // Get all files in the current directory
            string[] files = Directory.GetFiles(directoryPath);

            // Iterate through each file and log it
            foreach (var file in files)
            {
                Console.WriteLine($"Found file: {file}");

                // Ensure the directory exists for each file
                var fileDirectory = Path.GetDirectoryName(file);
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                    Console.WriteLine($"Created file directory: {fileDirectory}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing directory {directoryPath}: {ex.Message}");
        }
    
    }
    }
}
