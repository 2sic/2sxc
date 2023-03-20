using System.IO;
using System.Web.Hosting;

namespace ToSic.Sxc.Dnn.Install
{
    internal class DnnFileLock
    {
        internal string LockFileName => HostingEnvironment.MapPath(DnnConstants.LogDirectory + "lock.resources");
        internal string LockFolder => HostingEnvironment.MapPath(DnnConstants.LogDirectory);
        private FileStream _lockFile;

        internal FileStream Set()
        {
            return _lockFile ??
                   (_lockFile =
                       new FileStream(LockFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
        }

        internal void Release() => _lockFile?.Close();

        internal bool IsSet
        {
            get
            {
                Directory.CreateDirectory(LockFolder); // create if it doesn't exist (doesn't need checking beforehand)

                var lockFilePath = LockFileName;
                if (!File.Exists(lockFilePath))
                    return false;

                FileStream stream = null;
                try
                {
                    stream = new FileStream(lockFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                    stream.Close();
                    stream = null;
                    // delete doesn't seem to work... File.Delete(lockFilePath);
                }
                catch (IOException)
                {
                    // The file is unavailable because it is:
                    // - being processed by another thread
                    // - does not exist (has already been processed)
                    return true;
                }
                finally
                {
                    stream?.Close();
                }

                return false;
            }
        }
    }
}