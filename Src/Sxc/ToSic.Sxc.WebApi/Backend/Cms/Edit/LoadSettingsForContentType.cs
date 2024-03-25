using ToSic.Eav.Plumbing;
using static System.String;

namespace ToSic.Sxc.Backend.Cms;

internal class LoadSettingsForContentType()
    : LoadSettingsProviderBase($"{SxcLogName}.LdStCT"), ILoadSettingsProvider
{
    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters)
    {
        var l = Log.Fn<Dictionary<string, object>>();
        // find all keys which may be necessary
        var settingsKeys = parameters.ContentTypes
            .SelectMany(ct =>
                (ct.Metadata.DetailsOrNull?.AdditionalSettings ?? "").CsvToArrayWithoutEmpty()
            )
            .Where(c => !IsNullOrWhiteSpace(c))
            // Only include settings which have the full path
            // so in future we can add other roots like resources
            .Where(s => s.StartsWith($"{AppStackConstants.RootNameSettings}."))
            .ToList();

        // Try to find each setting
        var settings = SettingsByKeys(parameters.ContextOfApp.AppSettings, settingsKeys);

        return l.Return(settings, $"{settings.Count}");
    }
}