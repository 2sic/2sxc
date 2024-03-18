#if NETFRAMEWORK
using ToSic.Eav.Code.Infos;

namespace ToSic.Sxc.Apps
{
    public partial class App
    {
        [PrivateApi("obsolete, use the typed accessor instead, only included for old-compatibility")]
        [Obsolete("use the new, typed accessor instead")]
        dynamic SexyContent.Interfaces.IApp.Configuration
        {
            get
            {
                codeChanges.Value.Warn(OldIApp.Replace(nameof(Configuration)));
                var c = Configuration;
                return c?.Entity != null ? MakeDynProperty(c.Entity, propsRequired: false) : null;
            }
        }
        private static readonly ICodeInfo OldIApp = CodeInfoObsolete.V05To17("SexyContent.Interfaces.IApp.{0}",
            message: "Accessing the App.{0} through the ancient SexyContent.Interfaces.IApp interface is very deprecated.");

        dynamic SexyContent.Interfaces.IApp.Resources => codeChanges.Value.GetAndWarn(OldIApp.Replace(nameof(Resources)), Resources);

        dynamic SexyContent.Interfaces.IApp.Settings => codeChanges.Value.GetAndWarn(OldIApp.Replace(nameof(Settings)), Settings);

        string SexyContent.Interfaces.IApp.Path => codeChanges.Value.GetAndWarn(OldIApp.Replace(nameof(Path)), Path);

        string SexyContent.Interfaces.IApp.PhysicalPath => codeChanges.Value.GetAndWarn(OldIApp.Replace(nameof(PhysicalPath)), PhysicalPath);

        string SexyContent.Interfaces.IApp.Thumbnail => codeChanges.Value.GetAndWarn(OldIApp.Replace(nameof(Thumbnail)), Thumbnail);
    }
}

#endif