using ToSic.Eav.SysData;
using static ToSic.Eav.Internal.Features.BuiltInFeatures;

namespace ToSic.Sxc.Web.Internal.PageFeatures;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcPageFeatures
{
    /// <summary>
    /// JQuery feature
    /// </summary>
    /// <remarks>
    /// Published the key 'jQuery' in v12.02, do not change
    /// </remarks>
    public static PageFeature JQuery = new()
    {
        NameId = "jQuery",
        Name = "jQuery"
    };
        
    /// <summary>
    /// Internal feature, not published ATM
    /// </summary>
    public static PageFeature ContextPage = new()
    {
        NameId = "2sxc.ContextPage",
        Name = "the $2sxc headers in the page so everything works"
    };

    /// <summary>
    /// Internal feature, not published ATM
    /// </summary>
    public static PageFeature ContextModule = new()
    {
        NameId = "2sxc.ContextModule",
        Name = "the $2sxc headers in the module tag"
    };

    public static PageFeature JsApiOnModule = new()
    {
        NameId = "2sxc.JsApiOnModule",
        Name = "the $2sxc headers in the module tag"
    };

    /// <summary>
    /// The core 2sxc JS libraries
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.JsCore' in v13.00, do not change
    /// </remarks>
    public static PageFeature JsCore = new()
    {
        NameId = "2sxc.JsCore",
        Name = "2sxc core js APIs",
        Needs = [ContextPage.NameId],
        UrlInDist = "js/2sxc.api.min.js"
    };

    /// <summary>
    /// The INTERNAL USE 2sxc JS libraries for cms / edit actions.
    /// This one doesn't check requirements, and is the one which is added automatically. 
    /// </summary>
    public static PageFeature JsCmsInternal = new()
    {
        NameId = "Internal.JsCms",
        Name = "2sxc inpage editing APIs - internal version without checks",
        Needs =
        [
            JsCore.NameId,
            ContextModule.NameId
        ],
        UrlInDist = "dist/inpage/inpage.min.js"
    };

    private static readonly List<Requirement> RequiresPublicEditForm = [PublicEditForm.Requirement];

    /// <summary>
    /// The 2sxc JS libraries for cms / edit actions
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.JsCms' in v13.00, do not change
    /// </remarks>
    public static PageFeature JsCms = new()
    {
        NameId = "2sxc.JsCms",
        Name = "2sxc inpage editing APIs",
        Needs = [JsCmsInternal.NameId],
        Requirements = RequiresPublicEditForm
    };

    /// <summary>
    /// The 2sxc JS libraries for cms / edit actions
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.Toolbars' in v13.00, do not change
    /// </remarks>
    public static PageFeature ToolbarsInternal = new()
    {
        NameId = "Internal.Toolbars",
        Name = "2sxc InPage editing UIs / Toolbar",
        Needs =
        [
            JsCmsInternal.NameId,
            ContextPage.NameId,
        ],
        UrlInDist = "dist/inpage/inpage.min.css"
    };

    /// <summary>
    /// The 2sxc JS libraries for cms / edit actions
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.Toolbars' in v13.00, do not change
    /// </remarks>
    public static PageFeature Toolbars = new()
    {
        NameId = "2sxc.Toolbars",
        Name = "2sxc InPage editing UIs / Toolbar",
        Needs = [ToolbarsInternal.NameId],
        Requirements = RequiresPublicEditForm
    };

    /// <summary>
    /// WIP - this will probably be moved to local only in future, ATM it's global though
    /// </summary>
    public static PageFeature ToolbarsAutoInternal = new()
    {
        NameId = "Internal.ToolbarsAuto",
        Name = "Ensure that the toolbars automatically appear",
        Needs =
        [
            ContextPage.NameId,
            ToolbarsInternal.NameId
        ]
    };

    /// <summary>
    /// WIP - this will probably be moved to local only in future, ATM it's global though
    /// </summary>
    public static PageFeature ToolbarsAuto = new()
    {
        NameId = "2sxc.ToolbarsAuto",
        Name = "Ensure that the toolbars automatically appear",
        Needs = [ToolbarsAutoInternal.NameId],
        Requirements = RequiresPublicEditForm
    };

    /// <summary>
    /// turnOn feature
    /// </summary>
    /// <remarks>
    /// Published the key 'turnOn' in v12.02, do not change
    /// </remarks>
    public static PageFeature TurnOn = new()
    {
        NameId = "turnOn",
        Name = "turnOn JS library",
        UrlInDist = "dist/turnOn/turn-on.js"
    };

    /// <summary>
    /// turnOn feature
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    public static PageFeature CmsWysiwyg = new()
    {
        NameId = "Cms.Wysiwyg",
        Name = "Wysiwyg helpers / css for better rich content",
        UrlInDist = "dist/cms/wysiwyg.min.css"
    };

    /// <summary>
    /// Fancybox4 - just for use in the lightbox feature
    /// </summary>
    internal static readonly PageFeature WebResourceFancybox4 = new()
    {
        NameId = "fancybox4",
        Name = "Fancybox 4 lightbox"
    };

    public static PageFeature Lightbox = new()
    {
        NameId = "lightbox",
        Name = "The currently defined lightbox, whatever it is",
        Needs = [WebResourceFancybox4.NameId]
    };


    public static PageFeature EncryptFormData = new()
    {
        NameId = "Network.EncryptBody",
        Name = "Encrypts the form data before sending it to the server",
        Needs =
        [
            // Needs the 2sxc js-api on the module to work
            JsApiOnModule.NameId,
        ],
    };
}