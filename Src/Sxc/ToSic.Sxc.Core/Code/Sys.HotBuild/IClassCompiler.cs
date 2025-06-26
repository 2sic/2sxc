namespace ToSic.Sxc.Code.Sys.HotBuild;

public interface IClassCompiler
{
    object? InstantiateClass(string virtualPath, HotBuildSpec spec, string? className = null, string? relativePath = null, bool throwOnError = true);
}