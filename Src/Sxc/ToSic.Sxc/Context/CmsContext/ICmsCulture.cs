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
        /// </summary>
        /// <remarks>
        /// 1. It's always lower-case.
        /// 1. In the case of DNN, this corresponds to PortalSettings.DefaultCulture
        /// </remarks>
        string DefaultCode { get; }

        /// <summary>
        /// The current culture / language code like "de-ch". It's the language-code used by the translation environment. 
        /// </summary>
        /// <remarks>
        /// 1. It's always lower-case.
        /// 1. In the case of DNN, this corresponds to PortalSettings.CurrentAlias.CultureCode
        /// </remarks>
        string CurrentCode { get; }
    }
}
