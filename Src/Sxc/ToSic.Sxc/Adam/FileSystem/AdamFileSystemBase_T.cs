using ToSic.Lib.Services;

namespace ToSic.Sxc.Adam
{
    public abstract class AdamFileSystemBase: ServiceBase
    {
        public const int MaxSameFileRetries = 1000;
        public const int MaxUploadKbDefault = 25000;


        protected AdamFileSystemBase(string logPrefix) : base($"{logPrefix}.FilSys")
        {
        }
    }
}
