using System;
using System.IO;
using System.Runtime.InteropServices;

class Program
{
    // Made By killslvt || Make sure to run it as admin to get Temp & Prefetch

    [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
    public static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

    const uint SHERB_NOCONFIRMATION = 0x00000001;
    const uint SHERB_NOPROGRESSUI = 0x00000002;
    const uint SHERB_NOSOUND = 0x00000004;

    static void Main()
    {
        string userTempPath = GetFolderPath(Environment.SpecialFolder.UserProfile, "AppData", "Local", "Temp");
        string windowsTempPath = GetFolderPath(Environment.SpecialFolder.Windows, "Temp");
        string prefetchPath = GetFolderPath(Environment.SpecialFolder.Windows, "Prefetch");

        DisplayFolderPaths(userTempPath, windowsTempPath, prefetchPath);

        Console.WriteLine("[ = ] Press Enter To Delete Files In Folders");
        Console.ReadLine();

        DeleteFilesInFolder(userTempPath, "Temp");
        DeleteFilesInFolder(windowsTempPath, "%temp%");
        DeleteFilesInFolder(prefetchPath, "Prefetch");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n[ = ] Would you like to empty the Recycle Bin? (y/n)");
        Console.ResetColor();
        string response = Console.ReadLine();

        if (response.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ + ] Cleaning up the Recycle Bin Please Wait...");
            Console.ResetColor();
            EmptyRecycleBin();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ - ] Recycle Bin cleanup skipped.");
            Console.ResetColor();
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[ + ] Operation completed.");
        Console.ResetColor();
        Console.ReadLine();
    }

    static string GetFolderPath(Environment.SpecialFolder folder, params string[] subFolders)
    {
        return Path.Combine(Environment.GetFolderPath(folder), Path.Combine(subFolders));
    }

    static void DisplayFolderPaths(params string[] paths)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        try
        {
            foreach (var path in paths)
            {
                Console.WriteLine($"[Found {path}]");
            }
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var path in paths)
            {
                Console.WriteLine($"[Error Finding {path}]");
            }
        }
        Console.ResetColor();
    }

    static void DeleteFilesInFolder(string folderPath, string folderName)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[ = ] Deleting files from {folderName} folder: {folderPath}");
        Console.ResetColor();
        if (!Directory.Exists(folderPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ - ] Folder not found: {folderPath}");
            Console.ResetColor();
            return;
        }

        try
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                DeleteFile(file);
            }

            foreach (string subfolder in Directory.GetDirectories(folderPath))
            {
                DeleteFilesInFolder(subfolder, subfolder);
                Directory.Delete(subfolder);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[ + ] Deleted folder: " + subfolder);
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ + ] Deleted files and subfolders in folder: " + folderPath);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ - ] Error accessing folder: {folderPath}");
            Console.WriteLine($"[ - ] Error message: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }
    }

    static void DeleteFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ + ] Deleted file: " + filePath);
        }
        catch (UnauthorizedAccessException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ - ] Unauthorized access to delete file: " + filePath);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ - ] Error deleting file: {filePath}");
            Console.WriteLine($"[ - ] Error message: {ex.Message}");
        }
        finally
        {
            Console.ResetColor();
        }
    }

    static void EmptyRecycleBin()
    {
        int result = SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
        if (result == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ + ] Recycle Bin emptied successfully.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ - ] Failed to empty Recycle Bin. Error code: " + result);
        }
        Console.ResetColor();
    }
}
