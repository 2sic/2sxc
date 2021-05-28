using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Information about the cultures/languages used. 
    /// </summary>
    [PublicApi]
    public interface ICmsCulture
    {
        /// <summary>
        /// The default language code like "en-us" or "" (empty string).
        /// If the system is single-language, it will often just be an empty string "".
        /// <br/>
        /// <br/>
        /// Use in Razor: `CmsContext.Culture.DefaultCulture`
        /// </summary>
        /// <remarks>
        /// 1. It's always lower-case.
        /// 1. In the case of DNN, this corresponds to PortalSettings.DefaultCulture
        /// </remarks>
        string DefaultCode { get; }

        /// <summary>
        /// The current culture / language code like "de-ch". It's the language-code used by the translation environment. 
        /// <br/>
        /// <br/>
        /// Use in Razor: `CmsContext.Culture.CurrentCode`
        /// </summary>
        /// <remarks>
        /// 1. It's always lower-case.
        /// 1. In the case of DNN, this corresponds to PortalSettings.CurrentAlias.CultureCode
        /// </remarks>
        string CurrentCode { get; }
    }
}
