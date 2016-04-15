using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ToSic.SexyContent.Installer
{
    internal class Lock
    {
        internal string LockFileName => HostingEnvironment.MapPath(Settings.Installation.LogDirectory + "lock.resources");
        internal string LockFolder => HostingEnvironment.MapPath(Settings.Installation.LogDirectory);
        private FileStream _lockFile;

        internal FileStream Set()
            => _lockFile ?? (_lockFile = new FileStream(LockFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));

        internal void Release() => _lockFile?.Close();

        internal bool IsSet
        {
            get
            {
                //if (!Directory.Exists(LockFolder))
                Directory.CreateDirectory(LockFolder); // create if it doesn't exist (doesn't need checking beforehand)

                var lockFilePath = LockFileName;// HostingEnvironment.MapPath(Settings.Installation.LogDirectory + "lock.resources");
                if (!File.Exists(lockFilePath))
                    return false;

                FileStream stream = null;
                try
                {
                    stream = new FileStream(lockFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
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