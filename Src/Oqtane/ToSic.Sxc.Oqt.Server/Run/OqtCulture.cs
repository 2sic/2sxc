using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Oqtane.Infrastructure;
using Oqtane.Repository;
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

        /// <inheritdoc />
        public string DefaultCultureCode => _localizationManager.Value.GetDefaultCulture() ?? "en-us";

        // When culture code is not provided for selected default language, use "en-US".
        public string DefaultLanguageCode(int siteId) => (_languageRepository.Value.GetLanguages(siteId).FirstOrDefault(l => l.IsDefault)?.Code ?? "en-us");

        public  string CurrentCultureCode => CultureInfo.DefaultThreadCurrentUICulture?.Name ?? DefaultCultureCode;

        public List<TempTempCulture> GetSupportedCultures(int siteId)
        {
            //return new List<TempTempCulture>
            //{
            //    new TempTempCulture(WipConstants.DefaultLanguage, WipConstants.DefaultLanguageText, true)
            //};
            //return WipConstants.EmptyCultureList;

            // List of localizations enabled in Oqtane site.
            var siteCultures = _languageRepository.Value.GetLanguages(siteId)
                .Select(l => string.IsNullOrEmpty(l.Code) ? "en-US" : l.Code).ToList(); // default English is missing code in Oqtane v2.0.2.

            // All localizations that are installed in system (/code/Oqtane.Client.resources.dll) but default English is missing in the list.            
            var supportedCultures = _localizationManager.Value.GetSupportedCultures()
                .Union(new List<string>{"en-US"}).Distinct() // Add English, because it is missing in list in Oqtane v2.0.2.
                .Select(c => CultureInfo.GetCultureInfo(c))
                .Select(c => new TempTempCulture(c.Name.ToLowerInvariant(), c.EnglishName, siteCultures.Contains(c.Name, StringComparer.InvariantCultureIgnoreCase)))
                .ToList();
            
            return supportedCultures.OrderBy(c => c.Key).ToList();
        }

        public static void SetCulture(string culture)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}