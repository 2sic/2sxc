using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;
using ToSic.Sxc.Cms.Settings;
using ToSic.Sxc.Cms.Settings.InputFields;

namespace ToSic.Sxc.Backend.Cms.Load.Settings;

internal class LoadSettingsForWysiwygDefaults()
    : LoadSettingsForBase($"{SxcLogName}.LdGpsD", connect: [])
{
    public override Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) =>
        GetSettings<StringWysiwyg, StringWysiwyg>(
            parameters,
            StringWysiwyg.Defaults,
            false,
            StringWysiwyg.SettingsPath,
            StringWysiwyg.SettingsPath,
            model => model
        );
}