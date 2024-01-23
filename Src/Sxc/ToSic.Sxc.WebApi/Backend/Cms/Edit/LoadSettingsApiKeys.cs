using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Backend.Cms;

internal class LoadSettingsApiKeys(LazySvc<ISecureDataService> secureDataService)
    : LoadSettingsProviderBase($"{SxcLogging.SxcLogName}.StApiK"), ILoadSettingsProvider
{
    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) => Log.Func(l =>
    {
        var stack = parameters.ContextOfApp.AppSettings;

        var apiKeyNames = new List<string>
        {
            "Settings.GoogleMaps.ApiKey",
            "Settings.GoogleTranslate.ApiKey"
        };

        var result = apiKeyNames.Select(key =>
            {
                var prop = stack.InternalGetPath(key, Log);
                if (!(prop.Result is string strResult))
                    return null;

                var decrypted = secureDataService.Value.Parse(strResult);
                return new
                {
                    Key = key,
                    Value = new ContextApiKeyDto
                    {
                        NameId = key,
                        ApiKey = decrypted.Value,
                        IsDemo = decrypted.IsSecured
                    }
                };
            })
            .Where(v => v?.Value != null)
            .ToDictionary(k => k.Key, k => k.Value as object);

        return result;
    });
}