namespace ToSic.Sxc.Dnn.Install;

internal class Helpers
{
    /// <summary>
    /// Copy a Directory recursive
    /// </summary>
    /// <remarks>Source: http://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx </remarks>
    internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        try
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                // Skip if the file already exists in destination
                if (File.Exists(temppath))
                {
                    continue;
                }

                try
                {
                    file.CopyTo(temppath, false);
                }
                catch (IOException)
                {
                    // If copy fails (e.g., if file was created between our check and copy),
                    // just continue with the next file
                    continue;
                }
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (var subdir in dir.GetDirectories())
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

        }
        catch (UnauthorizedAccessException)
        {
            // Handle access denied errors gracefully
            // This could be logged or handled according to your requirements
            throw;
        }
        catch (Exception)
        {
            // Handle other unexpected errors
            // This could be logged or re-thrown according to your requirements
            throw;
        }
    }
}