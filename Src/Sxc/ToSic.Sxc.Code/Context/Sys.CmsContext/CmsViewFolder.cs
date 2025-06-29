﻿using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys.Assets;

namespace ToSic.Sxc.Context.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CmsViewFolder(CmsView cmsView, IApp app, bool shared) : AppAssetFolder
{
    [field: AllowNull, MaybeNull]
    public override string Path => field ??= FigureOutPath(shared ? app.RelativePathShared : app.RelativePath).Backslash();

    [field: AllowNull, MaybeNull]
    public override string Url => field ??= FigureOutPath(shared ? app.PathShared : app.Path);

    [field: AllowNull, MaybeNull]
    public override string PhysicalPath => field ??= FigureOutPath(shared ? app.PhysicalPathShared : app.PhysicalPath).Backslash();

    [field: AllowNull, MaybeNull]
    public override string Name => field ??= new DirectoryInfo(Path).Name;

    /// <summary>
    /// Figure out the path to the view based on a root path.
    /// </summary>
    /// <returns></returns>
    private string FigureOutPath(string root)
    {
        // Get addition, but must ensure it doesn't have a leading slash (otherwise Path.Combine treats it as a root)
        var addition = (cmsView.GetContents()?.EditionPath ?? "").TrimPrefixSlash();
        var pathWithFile = System.IO.Path.Combine(root ?? "", addition).ForwardSlash();
        return pathWithFile.BeforeLast("/");
    }
}