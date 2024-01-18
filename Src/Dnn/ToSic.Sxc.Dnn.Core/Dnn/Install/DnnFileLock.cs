using System.IO;
using System.Web.Hosting;

namespace ToSic.Sxc.Dnn.Install;

internal class DnnFileLock
{
    internal string LockFileName => HostingEnvironment.MapPath(DnnConstants.LogDirectory + "lock.resources");
    internal string LockFolder => HostingEnvironment.MapPath(DnnConstants.LogDirectory);
    private FileStream _lockFile;
    // Acquire lock
    internal FileStream Set() => _lockFile ??= new(LockFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

    // Close and dispose lock
    internal void Release()
    {
        _lockFile?.Close();
        _lockFile?.Dispose();
        _lockFile = null;
        try
        {
            // try delete
            File.Delete(LockFileName);
        }
        catch
        {
            // ignored
        }
    }

    internal bool IsSet
    {
        get
        {
            Directory.CreateDirectory(LockFolder); // create if it doesn't exist (doesn't need checking beforehand)

            var lockFilePath = LockFileName;
            if (!File.Exists(lockFilePath))
                return false;

            try
            {
                using (var stream = new FileStream(lockFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    // FileStream opened successfully, meaning the file is not locked
                }
                File.Delete(lockFilePath);
            }
            catch (IOException)
            {
                // The file is unavailable because it is:
                // - being processed by another thread
                // - does not exist (has already been processed)
                return true;
            }

            return false;
        }
    }
}