using ToSic.Eav.Internal.Catalogs;
using static ToSic.Sxc.Web.Internal.PageFeatures.SxcPageFeatures;

namespace ToSic.Sxc.Web.Internal.PageFeatures;

/// <summary>
/// Important: This is a singleton!
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PageFeaturesCatalog: GlobalCatalogBase<IPageFeature>
{
    /// <summary>
    /// Constructor - ATM we'll just add our known services here.
    ///
    /// </summary>
    /// <remarks>
    /// Important: if you want to add more services in a DI Startup, it must happen at Configure.
    /// If you do it earlier, the singleton retrieved then will not be the one at runtime.
    /// </remarks>
    public PageFeaturesCatalog(ILogStore logStore): base(logStore, SxcLogName + ".PftCat", new())
    {
        Register(
            JQuery,
            ContextPage,
            ContextModule,
            JsApiOnModule,
            JsCore,
            JsCms,
            JsCmsInternal,
            Toolbars,
            ToolbarsInternal,
            ToolbarsAuto,
            ToolbarsAutoInternal,
            TurnOn,
            CmsWysiwyg,
            Lightbox,
            EncryptFormData
        );
    }
}