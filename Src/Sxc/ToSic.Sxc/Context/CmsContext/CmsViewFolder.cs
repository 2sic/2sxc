using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Context;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsViewFolder: AppAssetFolder
{
    private readonly CmsView _cmsView;
    private readonly IApp _app;
    private readonly bool _shared;

    public CmsViewFolder(CmsView cmsView, IApp app, bool shared)
    {
        _cmsView = cmsView;
        _app = app;
        _shared = shared;
    }



    public override string Path => _path.Get(() => FigureOutPath(_shared ? _app.RelativePathShared : _app.RelativePath).Backslash());
    private readonly GetOnce<string> _path = new();

    public override string Url => _url.Get(() => FigureOutPath(_shared ? _app.PathShared : _app.Path));
    private readonly GetOnce<string> _url = new();

    public override string PhysicalPath => _physPath.Get(() => FigureOutPath(_shared ? _app.PhysicalPathShared : _app.PhysicalPath).Backslash());
    private readonly GetOnce<string> _physPath = new();

    public override string Name => _name ??= new DirectoryInfo(Path).Name;
    private string _name;

    /// <summary>
    /// Figure out the path to the view based on a root path.
    /// </summary>
    /// <returns></returns>
    private string FigureOutPath(string root)
    {
        // Get addition, but must ensure it doesn't have a leading slash (otherwise Path.Combine treats it as a root)
        var addition = (_cmsView.GetContents().EditionPath ?? "").TrimPrefixSlash();
        var pathWithFile = System.IO.Path.Combine(root ?? "", addition).ForwardSlash();
        return pathWithFile.BeforeLast("/");
    }
}