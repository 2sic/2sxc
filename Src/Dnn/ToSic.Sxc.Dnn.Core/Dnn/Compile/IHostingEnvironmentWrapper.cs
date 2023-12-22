using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn.Compile
{
    [PrivateApi]
    public interface IHostingEnvironmentWrapper
    {
        string MapPath(string virtualPath);
    }
}