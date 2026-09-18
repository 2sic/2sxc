namespace ToSic.Sxc.Code.Sys.HotBuild;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IClassCompiler
{
    object? InstantiateClass(string virtualPath, HotBuildSpec spec, string? className = null, string? relativePath = null, bool throwOnError = true);
}