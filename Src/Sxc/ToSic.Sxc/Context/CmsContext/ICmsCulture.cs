namespace ToSic.Sxc.Context;

/// <summary>
/// Information about the cultures/languages used.
/// 
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext.Culture`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyContext.Culture`
/// </summary>
[PublicApi]
public interface ICmsCulture
{
    /// <summary>
    /// The default language code like "en-us" or "" (empty string).
    /// If the system is single-language, it will often just be an empty string "".
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Culture.DefaultCode`  
    /// 🪒 Use in Typed Razor: `MyContext.Culture.DefaultCode`
    /// </summary>
    /// <remarks>
    /// 1. It's always lower-case.
    /// 1. In the case of DNN, this corresponds to PortalSettings.DefaultCulture
    /// </remarks>
    string DefaultCode { get; }

    /// <summary>
    /// The current culture / language code like "de-ch". It's the language-code used by the translation environment. 
    /// 
    /// 🪒 Use in Dynamic Razor: `CmsContext.Culture.CurrentCode`  
    /// 🪒 Use in Typed Razor: `MyContext.Culture.CurrentCode`
    /// </summary>
    /// <remarks>
    /// 1. It's always lower-case.
    /// 1. In the case of DNN, this corresponds to PortalSettings.CurrentAlias.CultureCode
    /// </remarks>
    string CurrentCode { get; }
}