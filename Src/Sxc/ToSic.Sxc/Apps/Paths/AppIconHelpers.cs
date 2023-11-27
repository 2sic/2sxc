using System.IO;
using ToSic.Eav.Data;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using static System.StringComparison;
using static ToSic.Eav.Apps.AppConstants;

namespace ToSic.Sxc.Apps.Paths;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppIconHelpers : ServiceBase
{
    #region Constructor / DI

    public AppIconHelpers(LazySvc<IValueConverter> iconConverterLazy): base("Viw.Help")
    {
        ConnectServices(
            _iconConverterLazy = iconConverterLazy
        );
    }
    private readonly LazySvc<IValueConverter> _iconConverterLazy;

    #endregion


    public string IconPathOrNull(IApp app, IView view, PathTypes type) => Log.Func(() =>
    {
        // 1. Check if the file actually exists or is a file:... reference
        var iconFile = IconPath(app, view, PathTypes.PhysFull);
        var assumeExists = ValueConverterBase.CouldBeReference(iconFile) || File.Exists(iconFile);

        // 2. Return as needed
        var result = assumeExists ? IconPath(app, view, type) : null;
        return (result, result ?? "not found");
    });

    private string IconPath(IApp app, IView view, PathTypes type)
    {
        // See if we have an icon - but only if we need the link
        if (view.Icon.HasValue())
        {
            // If we have the App:Path in front, replace as expected, but never on global
            if (HasAppPathToken(view.Icon))
                return AppPathTokenReplace(view.Icon, app.PathSwitch(false, type), app.PathSwitch(true, type));

            // If not, we must assume it's file:## placeholder
            // URLs (Links) we can provide...
            if (type == PathTypes.Link) return _iconConverterLazy.Value.ToValue(view.Icon, view.Guid);

            // ...but file:## can't convert to PhysFull; return it so the caller can check if it's a reference
            return view.Icon;
        }

        // Don't use the saved value, but return the expected path which would be the default by convention
        var viewPath1 = app.ViewPath(view, type);
        return viewPath1.Substring(0, viewPath1.LastIndexOf(".", Ordinal)) + ".png";
    }

    public static bool HasAppPathToken(string value)
    {
        value ??= "";
        return value.StartsWith(AppPathPlaceholder, InvariantCultureIgnoreCase)
               || value.StartsWith(AppPathSharedPlaceholder, InvariantCultureIgnoreCase);
    }

    public static string AppPathTokenReplace(string value, string appPath, string appPathShared)
    {
        value ??= "";
        if (value.StartsWith(AppPathPlaceholder, InvariantCultureIgnoreCase))
            return appPath + value.After(AppPathPlaceholder);
        if (value.StartsWith(AppPathSharedPlaceholder, InvariantCultureIgnoreCase))
            return appPathShared + value.After(AppPathSharedPlaceholder);
        return value;
    }
        
}