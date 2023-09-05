using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
// ReSharper disable UnusedMember.Global - we need these, as it's a public API

namespace ToSic.Sxc.Apps
{
    /// <summary>
    /// The configuration of the app, as you can set it in the app-package definition.
    /// </summary>
    [PublicApi]
    public class AppConfiguration: EntityBasedWithLog, IAppConfiguration
    {
        // todo: probably move most to Eav.Apps.AppConstants
        //[PrivateApi] public const string FieldDescription = "Description";
        //[PrivateApi] public const string FieldName = "DisplayName";
        //[PrivateApi] public const string FieldFolder = "Folder";
        //[PrivateApi] public const string FieldOriginalId = "OriginalId";
        //[PrivateApi] public const string FieldVersion = "Version";
        //[PrivateApi] public const string FieldHidden = "Hidden";
        [PrivateApi] public const string FieldAllowRazor = "AllowRazorTemplates";
        [PrivateApi] public const string FieldAllowToken = "AllowTokenTemplates";
        [PrivateApi] public const string FieldRequiredSxcVersion = "RequiredVersion";
        [PrivateApi] public const string FieldRequiredDnnVersion = "RequiredDnnVersion";
        [PrivateApi] public const string FieldRequiredOqtaneVersion = "RequiredOqtaneVersion";
        [PrivateApi] public const string FieldSupportsAjax = "SupportsAjaxReload";

        [PrivateApi]
        internal AppConfiguration(IEntity entity, ILog parentLog) : base(entity, parentLog, "Sxc.AppCnf")
        {
        }

        public Version Version => GetVersionOrDefault(nameof(Version));

        public string Name => Get(AppConstants.FieldName, Eav.Constants.NullNameId);

        public string Description => GetThis("");

        public string Folder => GetThis("");// Get( AppConstants.FieldFolder, "");

        public bool EnableRazor => Get(FieldAllowRazor, false);

        public bool EnableToken => Get(FieldAllowToken, false);

        public bool IsHidden => Get(AppConstants.FieldHidden, false);

        public bool EnableAjax => Get(FieldSupportsAjax, false);

        public Guid OriginalId => Guid.TryParse(GetThis(""), out var result) ? result : Guid.Empty;

        public Version RequiredSxc => GetVersionOrDefault(FieldRequiredSxcVersion);

        public Version RequiredDnn => GetVersionOrDefault(FieldRequiredDnnVersion);
        
        public Version RequiredOqtane => GetVersionOrDefault(FieldRequiredOqtaneVersion);

        private Version GetVersionOrDefault(string name) =>
            Version.TryParse(Get(name, ""), out var version) ? version : new Version();
    }
}
