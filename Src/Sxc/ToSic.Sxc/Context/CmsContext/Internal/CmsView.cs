using ToSic.Eav.Helpers;
using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsView(CmsContext parent, IBlock block) : CmsContextPartBase<IView>(parent, block.View), ICmsView
{
    private readonly IView _view = block.View;

    /// <inheritdoc />
    public int Id => _view?.Id ?? 0;

    /// <inheritdoc />
    public string Name => _view?.Name ?? "";

    /// <inheritdoc />
    public string Identifier => _view?.Identifier ?? "";

    /// <inheritdoc />
    public string Edition => _view?.Edition;

    protected override IMetadataOf GetMetadataOf()
        => ExtendWithRecommendations(_view?.Metadata);

    public IFolder Folder => _folder ??= FolderAdvanced();
    private IFolder _folder;

    [PrivateApi]
    private IFolder FolderAdvanced(NoParamOrder noParamOrder = default, string location = default)
        => new CmsViewFolder(this, block.App, AppAssetFolderMain.DetermineShared(location) ?? block.View.IsShared);

    /// <summary>
    /// Note: this is an explicit implementation, so in Dynamic Razor it won't work. This is by design.
    /// </summary>
    ITypedItem ICmsView.Settings => _settings.Get(() => Parent._CodeApiSvc.Cdf.AsItem(_view.Settings));
    private readonly GetOnce<ITypedItem> _settings = new();

    /// <summary>
    /// Note: this is an explicit implementation, so in Dynamic Razor it won't work. This is by design.
    /// </summary>
    ITypedItem ICmsView.Resources => _resources.Get(() => Parent._CodeApiSvc.Cdf.AsItem(_view.Resources));
    private readonly GetOnce<ITypedItem> _resources = new();

    /// <inheritdoc />
    [PrivateApi("Hidden in 16.04, because we want people to use the Folder. Can't remove it though, because there are many apps that already published this.")]
    public string Path => _path.Get(() => FigureOutPath(block?.App.Path));
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