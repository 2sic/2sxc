using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Context.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsViewFolder(CmsView cmsView, IApp app, bool shared) : AppAssetFolder
{
    public override string Path => _path.Get(() => FigureOutPath(shared ? app.RelativePathShared : app.RelativePath).Backslash());
    private readonly GetOnce<string> _path = new();

    public override string Url => _url.Get(() => FigureOutPath(shared ? app.PathShared : app.Path));
    private readonly GetOnce<string> _url = new();

    public override string PhysicalPath => _physPath.Get(() => FigureOutPath(shared ? app.PhysicalPathShared : app.PhysicalPath).Backslash());
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
        var addition = (cmsView.GetContents().EditionPath ?? "").TrimPrefixSlash();
        var pathWithFile = System.IO.Path.Combine(root ?? "", addition).ForwardSlash();
        return pathWithFile.BeforeLast("/");
    }
}