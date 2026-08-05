using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Render.Engines.Sys;

/// <summary>
/// WIP - trying to make engine more robust and dropping init
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record EngineSpecs
{
    public required IView View { get; init; }
    public required string TemplatePath { get; init; }
    public required string? Edition;
    public required IApp App { get; init; }
    public required IDataSource DataSource { get; init; }
    public required IBlock Block { get; init; }
    public required string? AppCacheKey { get; init; }

    public HotBuildSpec ToHotBuildSpec() => new(App.AppId, Edition, App.Name, AppCacheKey);
}
