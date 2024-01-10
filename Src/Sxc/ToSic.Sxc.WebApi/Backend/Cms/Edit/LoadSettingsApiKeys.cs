using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.WebApi.Cms;

internal class LoadSettingsApiKeys: LoadSettingsProviderBase, ILoadSettingsProvider
{
    private readonly LazySvc<ISecureDataService> _secureDataService;

    public LoadSettingsApiKeys(LazySvc<ISecureDataService> secureDataService) : base($"{SxcLogging.SxcLogName}.StApiK")
    {
        _secureDataService = secureDataService;
    }

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

                var decrypted = _secureDataService.Value.Parse(strResult);
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