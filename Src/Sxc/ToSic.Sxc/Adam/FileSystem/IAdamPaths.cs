using ToSic.Lib.Logging;

namespace ToSic.Sxc.Adam
{
    public interface IAdamPaths: IHasLog
    {
        IAdamPaths Init(AdamManager adamManager);

        string PhysicalPath(string path);

        string RelativeFromAdam(string path);

        string Url(string path);
    }
}
