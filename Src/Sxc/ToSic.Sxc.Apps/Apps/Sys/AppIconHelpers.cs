using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Views.Sys;
using ToSic.Eav.Data.Sys.ValueConverter;
using static System.StringComparison;

namespace ToSic.Sxc.Apps.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppIconHelpers(LazySvc<IValueConverter> iconConverterLazy)
    : ServiceBase("Viw.Help", connect: [iconConverterLazy])
{

    public string? IconPathOrNull(IAppPaths appPaths, IView view, PathTypes type)
    {
        var l = Log.Fn<string?>();
        // 1. Check if the file actually exists or is a file:... reference
        var iconFile = IconPath(appPaths, view, PathTypes.PhysFull);
        var assumeExists = ValueConverterBase.CouldBeReference(iconFile) || File.Exists(iconFile);

        // 2. Return as needed
        var result = assumeExists
            ? IconPath(appPaths, view, type)
            : null;
        return l.Return(result, result ?? "not found");
    }

    private string IconPath(IAppPaths appPaths, IView view, PathTypes type)
    {
        // See if we have an icon - but only if we need the link
        if (view.Icon.HasValue())
        {
            // If we have the App:Path in front, replace as expected, but never on global
            if (AppPathTokenHelper.HasAppPathToken(view.Icon))
                return AppPathTokenHelper.AppPathTokenReplace(view.Icon, appPaths.PathSwitch(false, type), appPaths.PathSwitch(true, type));

            // If not, we must assume it's file:## placeholder
            // URLs (Links) we can provide...
            if (type == PathTypes.Link)
                return iconConverterLazy.Value.ToValue(view.Icon, view.Guid);

            // ...but file:## can't convert to PhysFull; return it so the caller can check if it's a reference
            return view.Icon;
        }

        // Don't use the saved value, but return the expected path which would be the default by convention
        var viewPath1 = appPaths.ViewPath(view, type);
        return viewPath1.Substring(0, viewPath1.LastIndexOf(".", Ordinal)) + ".png";
    }

}
