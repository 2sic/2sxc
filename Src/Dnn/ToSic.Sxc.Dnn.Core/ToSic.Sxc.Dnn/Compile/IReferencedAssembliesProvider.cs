using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IReferencedAssembliesProvider
{
    List<string> Locations(string virtualPath, HotBuildSpec spec);
}