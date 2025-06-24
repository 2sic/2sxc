namespace ToSic.Sxc.Code.Internal.HotBuild;

public interface IClassCompiler
{
    object? InstantiateClass(string virtualPath, HotBuildSpec spec, string? className = null, string? relativePath = null, bool throwOnError = true);
}