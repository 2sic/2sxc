using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using ToSic.Eav.Apps.Languages;

namespace ToSic.Sxc.Oqt.Server.Context
{
    /// <summary>
    /// Manage oqtane site culture info
    /// </summary>
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
        public string DefaultCultureCode => MapTwoLetterCulture(_localizationManager.Value.GetDefaultCulture().ToLowerInvariant()) ?? FallbackLanguageCode;

        // When culture code is not provided for selected default language, use defaultLanguageCode.
        public string DefaultLanguageCode(int siteId)
        {
            return (_languageRepository.Value.GetLanguages(siteId).FirstOrDefault(l => l.IsDefault)?.Code ?? FallbackLanguageCode).ToLowerInvariant();
        }

        public string CurrentCultureCode => MapTwoLetterCulture(CultureInfo.CurrentCulture.Name).ToLowerInvariant();

        public List<ISiteLanguageState> GetSupportedCultures(int siteId, List<Eav.Data.DimensionDefinition>  availableEavLanguages)
        {
            var cultures = new List<string>(new[] { DefaultCultureCode });
            cultures.AddRange(_languageRepository.Value.GetLanguages(siteId).Select(language => MapTwoLetterCulture(language.Code)));

            // List of localizations enabled in Oqtane site.
            var siteCultures = cultures
                .Select(CultureInfo.GetCultureInfo)
                .Select(c => new SiteLanguageState(
                    c.Name.ToLowerInvariant(), 
                    c.EnglishName, 
                    availableEavLanguages.Any(a => a.Active && a.Matches(c.Name))
                    ))
                .ToList();
            
            return siteCultures
                .OrderByDescending(c => c.Code == DefaultLanguageCode(siteId))
                .Cast<ISiteLanguageState>()
                .ToList();
        }

        public static void SetCulture(string culture)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(MapTwoLetterCulture(culture));
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        public static string MapTwoLetterCulture(string culture)
        {
            if (string.IsNullOrEmpty(culture)) return FallbackLanguageCode;

            if (culture.Length > 3) return culture;

            // 1. For "en" return "en-us".
            if (culture.ToLowerInvariant() == "en") return FallbackLanguageCode;

            // 2. For other cultures first find is there simple de-de culture
            var simpleLanguageCode = CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .FirstOrDefault(c => c.Name.ToLowerInvariant() == $"{culture}-{culture}");

            if (simpleLanguageCode != null) return simpleLanguageCode.Name.ToLowerInvariant();

            // 3. If not, find first in list and return
            var firstLanguageCode = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .OrderBy(c => c.Name)
                .FirstOrDefault(c => c.TwoLetterISOLanguageName.ToLowerInvariant() == culture 
                                     && c.IsNeutralCulture == false);

            if (firstLanguageCode != null) return firstLanguageCode.Name.ToLowerInvariant();

            // 4. Fallback
            return FallbackLanguageCode;
        }
    }
}