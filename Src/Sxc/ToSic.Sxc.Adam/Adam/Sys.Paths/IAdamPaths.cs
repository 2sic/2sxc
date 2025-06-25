using ToSic.Sxc.Adam.Sys.Manager;

namespace ToSic.Sxc.Adam.Sys.Paths;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAdamPaths: IHasLog
{
    IAdamPaths Init(AdamManager adamManager);

    string PhysicalPath(string path);

    string RelativeFromAdam(string path);

    string Url(string path);
}