using ToSic.Eav.Logging;

namespace ToSic.Sxc.Adam
{
    public interface IAdamPaths
    {
        IAdamPaths Init(AdamManager adamManager, ILog parentLog);

        string PhysicalPath(string path);

        string PhysicalRelative(string path);
        
        //string PhysicalPath(IAsset asset);

        string Url(string path);
        
        //string Url(IAsset asset);
    }
}
