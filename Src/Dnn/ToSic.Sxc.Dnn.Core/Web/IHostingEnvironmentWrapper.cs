using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Web
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice()]
    public interface IHostingEnvironmentWrapper
    {
        string MapPath(string virtualPath);
    }
}