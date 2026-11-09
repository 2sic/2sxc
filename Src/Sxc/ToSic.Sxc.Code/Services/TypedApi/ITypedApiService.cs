using ToSic.Eav.Context;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
#pragma warning disable IDE0051

namespace ToSic.Sxc.Services;

/// <summary>
/// This is a service to give you Typed Code outside 2sxc.
/// </summary>
/// <remarks>
/// Use this to access 2sxc data from a Theme, a `.ascx` WebControl or anywhere else. 
/// 
/// History
/// 
/// * New in v20, as a replacement for the [IDynamicCodeService](xref:ToSic.Sxc.Services.IDynamicCodeService)
/// </remarks>
[PublicApi]
public interface ITypedApiService
{
    /// <summary>
    /// Get the Typed App object for a specific App, which is the most convenient way to work with the content of this App.
    /// </summary>
    /// <returns></returns>
    IAppTyped App(NoParamOrder npo = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null);

    /// <summary>
    /// Get the Typed App object for the site-wide App, which contains shared data.
    /// </summary>
    /// <returns></returns>
    IAppTyped AppOfSite();

    /// <summary>
    /// Get an API object for a specific App.
    /// </summary>
    /// <returns></returns>
    ITypedApi ApiOfApp(int appId);

    /// <summary>
    /// Get an API object for a specific App.
    /// </summary>
    /// <returns></returns>
    ITypedApi ApiOfApp(int zoneId, int appId);

    /// <summary>
    /// Get an API object for a specific Module - the Module must have a 2sxc App or Content on it for this to work.
    /// </summary>
    /// <remarks>
    /// This is to work with the content, data or app on this specific module.
    /// It is *not* meant for module-level metadata, which should be on the ApiOfSite.
    /// </remarks>
    /// <returns></returns>
    ITypedApi ApiOfModule(int pageId, int moduleId);


    /// <summary>
    /// Get an API object for a current Site/Portal.
    /// </summary>
    /// <returns></returns>
    ITypedApi ApiOfSite();

    /// <summary>
    /// Get an API object for a specified Site/Portal.
    /// </summary>
    /// <returns></returns>
    ITypedApi ApiOfSite(int siteId);

    /// <summary>
    /// Get an API object for a specific Site/Portal including proper context of a specific Module on a page.
    /// </summary>
    /// <remarks>
    /// This is the most complex way to get an API, but it will give you the most complete context for your code, which you need for ModuleMetadata.
    /// </remarks>
    /// <returns></returns>
    ITypedApi ApiOfSite(int siteId, int pageId, int moduleId);


}