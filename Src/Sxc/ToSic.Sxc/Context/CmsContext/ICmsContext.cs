namespace ToSic.Sxc.Context;

/// <summary>
/// This is the runtime context of your code in the CMS. It can tell you about the site, page, module etc. that you're on.
/// Note that it it _Platform Agnostic_ so it's the same on Dnn, Oqtane etc.
///
/// 🪒 In [Dynamic Razor](xref:Custom.Hybrid.Razor14) it's found on `CmsContext`  
/// 🪒 In [Typed Razor](xref:Custom.Hybrid.RazorTyped) it's found on `MyContext`, but many objects are directly available, like `MyPage`
/// </summary>
[PublicApi]
public interface ICmsContext
{
    /// <summary>
    /// Information about languages / culture of the current request
    /// </summary>
    ICmsCulture Culture { get; }

    /// <summary>
    /// Information about the Module / Container which holds a 2sxc content block in the CMS
    /// </summary>
    ICmsModule Module { get; }

    /// <summary>
    /// Information about the current Page (called Tab in DNN).
    /// It's especially useful to get current URL Parameters.
    /// </summary>
    ICmsPage Page { get; }

    /// <summary>
    /// Information about the platform that's currently running.
    /// </summary>
    ICmsPlatform Platform { get; }

    /// <summary>
    /// Information about the Site (called Portal in DNN)
    /// </summary>
    ICmsSite Site { get; }

    /// <summary>
    /// Information about the current user.
    /// It's especially useful to see if the user has any kind of Admin privileges.
    /// </summary>
    ICmsUser User { get; }
        
        
    /// <summary>
    /// View-information such as the view Name, Identity or Edition.
    /// </summary>
    /// <remarks>New in v12.02</remarks>
    ICmsView View { get; }


    /// <summary>
    /// Information about the current block, similar to the module but a bit different.
    /// </summary>
    /// <remarks>
    /// Added ca. v13, but not documented/published till v17.
    /// </remarks>
    ICmsBlock Block { get; }
}