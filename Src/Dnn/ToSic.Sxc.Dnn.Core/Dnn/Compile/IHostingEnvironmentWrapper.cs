namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IHostingEnvironmentWrapper
{
    string MapPath(string virtualPath);
}