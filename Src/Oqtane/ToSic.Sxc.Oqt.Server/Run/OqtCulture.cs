using Oqtane.Infrastructure;
using Oqtane.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtCulture
    {
        public OqtCulture(Lazy<ILocalizationManager> localizationManager, Lazy<ILanguageRepository> languageRepository)
        {
            _localizationManager = localizationManager;
            _languageRepository = languageRepository;
        }
        private readonly Lazy<ILocalizationManager> _localizationManager;
        private readonly Lazy<ILanguageRepository> _languageRepository;

        const string FallbackLanguageCode = "en-us";
        /// <inheritdoc />
        public string DefaultCultureCode => _localizationManager.Value.GetDefaultCulture().ToLowerInvariant() ?? FallbackLanguageCode;

        // When culture code is not provided for selected default language, use defaultLanguageCode.
        public string DefaultLanguageCode(int siteId)
        {

            return (_languageRepository.Value.GetLanguages(siteId).FirstOrDefault(l => l.IsDefault)?.Code ?? FallbackLanguageCode).ToLowerInvariant();
        }

        public string CurrentCultureCode => CultureInfo.DefaultThreadCurrentUICulture?.Name.ToLowerInvariant() ?? DefaultCultureCode;

        public List<TempTempCulture> GetSupportedCultures(int siteId, List<DimensionDefinition>  availableEavLanguages)
        {
            // List of localizations enabled in Oqtane site.
            var siteCultures = _languageRepository.Value.GetLanguages(siteId)
                .Select(l => string.IsNullOrEmpty(l.Code) ? FallbackLanguageCode : l.Code).ToList() // default English is missing code in Oqtane v2.0.2.
                .Select(c => CultureInfo.GetCultureInfo(c))
                .Select(c => new TempTempCulture(
                    c.Name.ToLowerInvariant(), 
                    c.EnglishName, 
                    availableEavLanguages.Any(a => a.Active && a.Matches(c.Name))
                    ))
                .ToList();
            
            return siteCultures.OrderByDescending(c => c.Key == DefaultLanguageCode(siteId)).ToList();
        }

        public static void SetCulture(string culture)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}