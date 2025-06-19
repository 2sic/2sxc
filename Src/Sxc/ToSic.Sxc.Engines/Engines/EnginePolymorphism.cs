﻿using ToSic.Eav.Apps;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EnginePolymorphism(PolymorphConfigReader polymorphism, IServerPaths serverPaths)
    : ServiceBase("Sxc.EngPly", connect: [polymorphism, serverPaths])
{
    internal (string? NewPath, string? Edition) PolymorphTryToSwitchPath(string root, IView view, IAppReader appReader)
    {
        var subPath = view.Path;
        var l = Log.Fn<(string? NewPath, string? Edition)>($"{root}, {subPath}");
        // Get initial path - here the file is already reliably stored
        view.EditionPath = subPath.ToAbsolutePathForwardSlash();
        subPath = view.EditionPath.TrimPrefixSlash();

        // Figure out the current edition - if none, stop here
        // New 2023-03-20 - if the view comes with a preset edition, it's an ajax-preview which should be respected
        var edition = polymorphism.UseViewEditionOrGet(view, appReader);
        
        if (edition == null)
            return l.Return((null, null), "no edition detected");
        l.A($"edition '{edition}' detected");

        // Case #1 where edition is between root and path
        // like subPath = "View.cshtml" and there is a "bs5/View.cshtml"
        var newPath = PolymorphTestPathAndSetIfFound(view, root, edition, subPath);
        if (newPath != null)
            return l.Return((newPath, edition), $"found edition {edition}");

        // Case #2 where edition _replaces_ an edition in the current path
        // like subPath ="bs5/View.cshtml" and there is a "bs4/View.cshtml"
        l.A("tried inserting path, will check if sub-path");
        var pathWithoutFirstFolder = subPath.After("/");
        if (pathWithoutFirstFolder.IsEmpty())
            return l.ReturnNull("view is not in subfolder, so no edition to replace, stopping now");
        newPath = PolymorphTestPathAndSetIfFound(view, root, edition, pathWithoutFirstFolder);
        if (newPath != null)
            return l.Return((newPath, edition), $"edition {edition} up one path");

        return l.ReturnNull($"edition {edition} not found");
    }

    private string? PolymorphTestPathAndSetIfFound(IView view, string root, string edition, string subPath)
    {
        var l = Log.Fn<string>($"root: {root}; edition: {edition}; subPath: {subPath}");
        var fullPathForTest = Path.Combine(root, edition, subPath).ToAbsolutePathForwardSlash();
        if (!File.Exists(serverPaths.FullAppPath(fullPathForTest)))
            return l.ReturnNull("not found");
        view.Edition = edition;
        view.EditionPath = Path.Combine(edition, subPath).ToAbsolutePathForwardSlash();
        return l.Return(fullPathForTest, $"edition {edition}");
    }

}