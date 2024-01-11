using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Services;

/// <summary>
/// This is a service to give you DynamicCode outside of 2sxc.
///
/// Use this to access 2sxc data from a Theme, a `.ascx` WebControl or anywhere else. 
/// </summary>
/// <remarks>
/// * New in v13.02
/// * This is meant to replace the `ToSic.Sxc.Dnn.Factory`. Please use this from now on.
/// </remarks>
[PublicApi]
public interface IDynamicCodeService
{
    /// <summary>
    /// Get a <see cref="IDynamicCode12"/> object for a specific Module on a page
    /// </summary>
    /// <param name="pageId"></param>
    /// <param name="moduleId"></param>
    /// <returns></returns>
    IDynamicCode12 OfModule(int pageId, int moduleId);

    /// <summary>
    /// Get a <see cref="IDynamicCode12"/> object for the primary App of the current Site/Portal.
    /// </summary>
    /// <returns></returns>
    IDynamicCode12 OfSite();

    /// <summary>
    /// Get a <see cref="IDynamicCode12"/> object for a specific Site/Portal.
    /// </summary>
    /// <returns></returns>
    IDynamicCode12 OfSite(int siteId);


    /// <summary>
    /// Get a <see cref="IDynamicCode12"/> object for a specific App.
    /// This is the simplest way to work with Dynamic Code for this App.
    ///
    /// Note that this is without Page/Module context, so there will be no useful `Content` object on the dynamic code.
    /// </summary>
    /// <param name="appId">The AppId</param>
    /// <returns>The dynamic code object for this App</returns>
    IDynamicCode12 OfApp(int appId);

    /// <summary>
    /// Get a <see cref="IDynamicCode12"/> object for a specific App.
    /// This is the simplest way to work with Dynamic Code for this App.
    /// 
    /// Note that this is without Page/Module context, so there will be no useful `Content` object on the dynamic code.
    /// </summary>
    /// <param name="zoneId">The ZoneId of the App</param>
    /// <param name="appId">The AppId</param>
    /// <returns>The dynamic code object for this App</returns>
    IDynamicCode12 OfApp(int zoneId, int appId);

    /// <summary>
    /// Get a <see cref="IDynamicCode12"/> object for a specific App.
    /// This is the simplest way to work with Dynamic Code for this App.
    /// 
    /// Note that this is without Page/Module context, so there will be no useful `Content` object on the dynamic code.
    /// </summary>
    /// <param name="appIdentity">The App Identifier</param>
    /// <returns>The dynamic code object for this App</returns>
    IDynamicCode12 OfApp(IAppIdentity appIdentity);

    /// <summary>
    /// Get a rich <see cref="Apps.IApp"/> object for a specific App.
    /// This is the simplest way to work with data of this App, but won't give you commands like `AsDynamic(...)`
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="zoneId">The ZoneId of the App (optional)</param>
    /// <param name="appId">The AppId - REQUIRED</param>
    /// <param name="site">Site information for further context (optional) </param>
    /// <param name="withUnpublished">Determines if the App.Data gives you unpublished data (like in admin-mode) or just published data. If not set, will default to user permissions.</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    IApp App(
        NoParamOrder noParamOrder = default,
        int? zoneId = null,
        int? appId = null,
        ISite site = null,
        bool? withUnpublished = null);

    /// <summary>
    /// Get a rich <see cref="Apps.IApp"/> object for the primary (aka Site) App of the _current_ Site/Portal and using permissions of the current user.
    /// This is the simplest way to work with data of this App, but won't give you commands like `AsDynamic(...)`.
    /// For more control, use the `AppOfSite` with parameters. 
    /// </summary>
    IApp AppOfSite();
    
    /// <summary>
    /// Get a rich <see cref="Apps.IApp"/> object for the primary (aka Site) App. 
    /// This is the simplest way to work with data of this App, but won't give you commands like `AsDynamic(...)`
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="siteId">The SiteId which is the PortalId in DNN. Optional - without it, the current Site / Portal is used. </param>
    /// <param name="site">Site information for further context (optional) </param>
    /// <param name="withUnpublished">Determines if the App.Data gives you unpublished data (like in admin-mode) or just published data. If not set, will default to user permissions.</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    IApp AppOfSite(
        // ReSharper disable once MethodOverloadWithOptionalParameter
        NoParamOrder noParamOrder = default,
        int? siteId = null,
        ISite site = null,
        bool? withUnpublished = null);
}