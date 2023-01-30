using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.WebApi.Cms
{
    public class LoadSettingsApiKeys: LoadSettingsProviderBase, ILoadSettingsProvider
    {
        private readonly LazySvc<ISecureDataService> _secureDataService;

        public LoadSettingsApiKeys(LazySvc<ISecureDataService> secureDataService) : base($"{Constants.SxcLogName}.StApiK")
        {
            _secureDataService = secureDataService;
        }

        public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) => Log.Func(l =>
        {
            var stack = parameters.ContextOfApp.AppSettings;

            //var parts = new Dictionary<string, string>
            //{
            //    { "Settings.GoogleMaps.ApiKey", "google-maps" },
            //    { "Settings.GoogleTranslate.ApiKey", "google-translate" }
            //};
            var keys = new []
            {
                "Settings.GoogleMaps.ApiKey",
                "Settings.GoogleTranslate.ApiKey"
            };

            var result = keys.Select(key =>
                {
                    var prop = stack.InternalGetPath(new PropReqSpecs(key, Array.Empty<string>(), Log),
                        new PropertyLookupPath());

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
                            IsDemo = decrypted.IsSecure
                        }
                    };
                })
                .Where(v => v?.Value != null)
                .ToDictionary(k => k.Key, k => k.Value as object);

            return result;
        });
    }
}
