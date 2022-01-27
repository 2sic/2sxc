using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using static System.StringComparison;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Engines
{

    public class AppIconHelpers : HasLog<AppIconHelpers>
    {
        #region Constructor / DI

        public AppIconHelpers(Lazy<IValueConverter> iconConverterLazy): base("Viw.Help") => _iconConverterLazy = iconConverterLazy;
        private readonly Lazy<IValueConverter> _iconConverterLazy;

        #endregion
        

        public string IconPathOrNull(IApp app, IView view, PathTypes type)
        {
            var wrapLog = Log.Call<string>();
            // 1. Check if the file actually exists
            var iconFile = IconPath(app, view, PathTypes.PhysFull);
            var exists = File.Exists(iconFile);

            // 2. Return as needed
            var result = exists ? IconPath(app, view, type) : null;
            return wrapLog(result ?? "not found", result);
        }

        private string IconPath(IApp app, IView view, PathTypes type)
        {
            // See if we have an icon - but only if we need the link
            if(!string.IsNullOrWhiteSpace(view.Icon))
            {
                var iconInConfig = view.Icon;
                
                // If we have the App:Path in front, replace as expected, but never on global
                if (HasAppPathToken(iconInConfig))
                    return AppPathTokenReplace(iconInConfig, app.PathSwitch(false, type), app.PathSwitch(true, type));

                // If not, we must assume it's file:## placeholder and we can only convert to relative link
                if (type == PathTypes.Link) return _iconConverterLazy.Value.ToValue(iconInConfig, view.Guid);

                // Otherwise ignore the request and proceed by standard
            }

            var viewPath1 = app.ViewPath(view, type);
            return viewPath1.Substring(0, viewPath1.LastIndexOf(".", Ordinal)) + ".png";
        }

        public static bool HasAppPathToken(string value)
        {
            value = value ?? "";
            return value.StartsWith(AppConstants.AppPathPlaceholder, InvariantCultureIgnoreCase)
                || value.StartsWith(AppConstants.AppPathSharedPlaceholder, InvariantCultureIgnoreCase);
        }

        public static string AppPathTokenReplace(string value, string appPath, string appPathShared)
        {
            value = value ?? "";
            if (value.StartsWith(AppConstants.AppPathPlaceholder, InvariantCultureIgnoreCase))
                return appPath + value.After(AppConstants.AppPathPlaceholder);
            if (value.StartsWith(AppConstants.AppPathSharedPlaceholder, InvariantCultureIgnoreCase))
                return appPathShared + value.After(AppConstants.AppPathSharedPlaceholder);
            return value;
        }
        
    }
}