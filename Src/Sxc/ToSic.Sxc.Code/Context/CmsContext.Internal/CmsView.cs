using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.Internal.Assets;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Sys.ExecutionContext;
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace ToSic.Sxc.Context.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CmsView(CmsContext parent, IBlock block) : CmsContextPartBase<IView>(parent, block.View!), ICmsView
{
    private readonly IView _view = block.View!;

    /// <inheritdoc />
    public int Id => _view?.Id ?? 0;

    /// <inheritdoc />
    public string Name => _view?.Name ?? "";

    /// <inheritdoc />
    public string Identifier => _view?.Identifier ?? "";

    /// <inheritdoc />
    public string Edition => _view?.Edition ?? "";

    protected override IMetadata GetMetadataOf()
        => _view?.Metadata.AddRecommendations()!;

    [field: AllowNull, MaybeNull]
    public IFolder Folder => field ??= FolderAdvanced();

    [PrivateApi]
    private IFolder FolderAdvanced(NoParamOrder noParamOrder = default, string? location = default)
        => new CmsViewFolder(this, block.App, AppAssetsHelpers.DetermineShared(location) ?? _view.IsShared);

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory Cdf => field ??= Parent.ExCtx.GetCdf();

    /// <summary>
    /// Note: this is an explicit implementation, so in Dynamic Razor it won't work. This is by design.
    /// </summary>
    ITypedItem? ICmsView.Settings => _settings.Get(() => Cdf.AsItem(_view.Settings, new() { ItemIsStrict = true }));
    private readonly GetOnce<ITypedItem?> _settings = new();

    /// <summary>
    /// Note: this is an explicit implementation, so in Dynamic Razor it won't work. This is by design.
    /// </summary>
    ITypedItem? ICmsView.Resources => _resources.Get(() => Cdf.AsItem(_view.Resources, new() { ItemIsStrict = true }));
    private readonly GetOnce<ITypedItem?> _resources = new();

    /// <inheritdoc />
    [PrivateApi("Hidden in 16.04, because we want people to use the Folder. Can't remove it though, because there are many apps that already published this.")]
    public string Path => _path.Get(() => FigureOutPath(block.App.Path))!;
    private readonly GetOnce<string> _path = new();

    /// <summary>
    /// Figure out the path to the view based on a root path.
    /// </summary>
    /// <returns></returns>
    private string FigureOutPath(string root)
    {
        // Get addition, but must ensure it doesn't have a leading slash (otherwise Path.Combine treats it as a root)
        var addition = (_view.EditionPath ?? "").TrimPrefixSlash();
        var pathWithFile = System.IO.Path.Combine(root ?? "", addition).ForwardSlash();
        return pathWithFile.BeforeLast("/");
    }
}