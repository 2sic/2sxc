using ToSic.Eav.Helpers;
using ToSic.Eav.Metadata;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CmsView: CmsContextPartBase<IView>, ICmsView
{
    public CmsView(CmsContext parent, IBlock block) : base(parent, block.View)
    {
        _block = block;
    }

    private readonly IBlock _block;

    /// <inheritdoc />
    public int Id => GetContents()?.Id ?? 0;

    /// <inheritdoc />
    public string Name => GetContents()?.Name ?? "";

    /// <inheritdoc />
    public string Identifier => GetContents()?.Identifier ?? "";

    /// <inheritdoc />
    public string Edition => GetContents()?.Edition;

    protected override IMetadataOf GetMetadataOf()
        => ExtendWithRecommendations(GetContents()?.Metadata);

    public IFolder Folder => _folder ??= FolderAdvanced();
    private IFolder _folder;

    [PrivateApi]
    IFolder FolderAdvanced(NoParamOrder noParamOrder = default, string location = default)
    {
        return new CmsViewFolder(this, _block.App, AppAssetFolderMain.DetermineShared(location) ?? _block.View.IsShared);
    }


    /// <inheritdoc />
    public string Path => _path.Get(() => FigureOutPath(_block?.App.Path));
    private readonly GetOnce<string> _path = new();

    ///// <inheritdoc />
    //public string PathShared => _pathShared.Get(() => FigureOutPath(_block?.App.PathShared));
    //private readonly GetOnce<string> _pathShared = new GetOnce<string>();

    ///// <inheritdoc />
    //public string PhysicalPath => _physPath.Get(() => FigureOutPath(_block?.App.PhysicalPath));
    //private readonly GetOnce<string> _physPath = new GetOnce<string>();

    ///// <inheritdoc />
    //public string PhysicalPathShared => _physPathShared.Get(() => FigureOutPath(_block?.App.PhysicalPathShared));
    //private readonly GetOnce<string> _physPathShared = new GetOnce<string>();

    /// <summary>
    /// Figure out the path to the view based on a root path.
    /// </summary>
    /// <returns></returns>
    private string FigureOutPath(string root)
    {
        // Get addition, but must ensure it doesn't have a leading slash (otherwise Path.Combine treats it as a root)
        var addition = (GetContents().EditionPath ?? "").TrimPrefixSlash();
        var pathWithFile = System.IO.Path.Combine(root ?? "", addition).ForwardSlash();
        return pathWithFile.BeforeLast("/");
    }
}