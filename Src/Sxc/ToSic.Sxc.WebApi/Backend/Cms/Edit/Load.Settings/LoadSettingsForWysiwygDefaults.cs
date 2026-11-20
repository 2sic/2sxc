using ToSic.Eav.Ui.Edit.Sys.Load.Settings;
using ToSic.Sxc.Cms.Settings.InputFields;

namespace ToSic.Sxc.Backend.Cms.Load.Settings;

internal class LoadSettingsForWysiwygDefaults()
    : ServiceBase($"{SxcLogName}.LdGpsD", connect: []), ILoadSettingsProvider
{
    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) =>
        LoadSettingsForBase.GetSettings<StringWysiwyg, StringWysiwyg>(
            appSettings: parameters.ContextOfApp.AppSettings,
            defaults: StringWysiwyg.Defaults,
            forceDefaults: false,
            lookupPath: StringWysiwyg.SettingsPath,
            outputPath: StringWysiwyg.SettingsPath,
            modelConverter: model => model,
            log: Log);
}