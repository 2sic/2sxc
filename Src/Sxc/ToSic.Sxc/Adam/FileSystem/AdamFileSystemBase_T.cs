using ToSic.Lib.Logging;

namespace ToSic.Sxc.Adam
{
    public abstract class AdamFileSystemBase<TFolder, TFile>: HasLog
    {
        public const int MaxSameFileRetries = 1000;
        public const int MaxUploadKbDefault = 25000;


        protected AdamFileSystemBase(string logPrefix) : base($"{logPrefix}.FilSys")
        {
        }
    }
}
