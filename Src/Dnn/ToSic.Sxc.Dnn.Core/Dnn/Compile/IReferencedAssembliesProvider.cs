using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
public interface IReferencedAssembliesProvider
{
    List<string> Locations(string virtualPath, HotBuildSpec spec);
}