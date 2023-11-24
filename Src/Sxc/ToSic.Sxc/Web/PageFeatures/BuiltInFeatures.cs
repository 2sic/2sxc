using System.Collections.Generic;
using ToSic.Eav.SysData;
using static ToSic.Eav.Internal.Features.BuiltInFeatures;

namespace ToSic.Sxc.Web.PageFeatures;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BuiltInFeatures
{
    /// <summary>
    /// JQuery feature
    /// </summary>
    /// <remarks>
    /// Published the key 'jQuery' in v12.02, do not change
    /// </remarks>
    public static PageFeature JQuery = new("jQuery", "jQuery");
        
    /// <summary>
    /// Internal feature, not published ATM
    /// </summary>
    public static PageFeature ContextPage = new("2sxc.ContextPage", "the $2sxc headers in the page so everything works");

    /// <summary>
    /// Internal feature, not published ATM
    /// </summary>
    public static PageFeature ContextModule = new("2sxc.ContextModule", "the $2sxc headers in the module tag");

    /// <summary>
    /// The core 2sxc JS libraries
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.JsCore' in v13.00, do not change
    /// </remarks>
    public static PageFeature JsCore = new("2sxc.JsCore", "2sxc core js APIs", needs: new[]
    {
        ContextPage.NameId
    }, urlWip: "js/2sxc.api.min.js");

    /// <summary>
    /// The INTERNAL USE 2sxc JS libraries for cms / edit actions.
    /// This one doesn't check requirements, and is the one which is added automatically. 
    /// </summary>
    public static PageFeature JsCmsInternal = new("Internal.JsCms", "2sxc inpage editing APIs - internal version without checks", needs: new[]
    {
        JsCore.NameId,
        ContextModule.NameId,
    }, urlWip: "dist/inpage/inpage.min.js");

    private static readonly List<Requirement> RequiresPublicEditForm = new() { PublicEditForm.Requirement };

    /// <summary>
    /// The 2sxc JS libraries for cms / edit actions
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.JsCms' in v13.00, do not change
    /// </remarks>
    public static PageFeature JsCms = new("2sxc.JsCms", 
        "2sxc inpage editing APIs", 
        needs: new[] { JsCmsInternal.NameId },
        requirements: RequiresPublicEditForm);

    /// <summary>
    /// The 2sxc JS libraries for cms / edit actions
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.Toolbars' in v13.00, do not change
    /// </remarks>
    public static PageFeature ToolbarsInternal = new("Internal.Toolbars",
        "2sxc InPage editing UIs / Toolbar",
        needs: new[]
        {
            JsCmsInternal.NameId,
            ContextPage.NameId,
        }, urlWip: "dist/inpage/inpage.min.css");

    /// <summary>
    /// The 2sxc JS libraries for cms / edit actions
    /// </summary>
    /// <remarks>
    /// Published the key '2sxc.Toolbars' in v13.00, do not change
    /// </remarks>
    public static PageFeature Toolbars = new("2sxc.Toolbars",
        "2sxc InPage editing UIs / Toolbar",
        needs: new[] { ToolbarsInternal.NameId },
        requirements: RequiresPublicEditForm);

    /// <summary>
    /// WIP - this will probably be moved to local only in future, ATM it's global though
    /// </summary>
    public static PageFeature ToolbarsAutoInternal = new("Internal.ToolbarsAuto",
        "Ensure that the toolbars automatically appear", needs: new[]
        {
            ContextPage.NameId,
            ToolbarsInternal.NameId,
        });

    /// <summary>
    /// WIP - this will probably be moved to local only in future, ATM it's global though
    /// </summary>
    public static PageFeature ToolbarsAuto = new("2sxc.ToolbarsAuto",
        "Ensure that the toolbars automatically appear",
        needs: new[] { ToolbarsAutoInternal.NameId },
        requirements: RequiresPublicEditForm);

    /// <summary>
    /// turnOn feature
    /// </summary>
    /// <remarks>
    /// Published the key 'turnOn' in v12.02, do not change
    /// </remarks>
    public static PageFeature TurnOn = new("turnOn", "turnOn JS library", urlWip: "dist/turnOn/turn-on.js");

    /// <summary>
    /// turnOn feature
    /// </summary>
    /// <remarks>
    /// Added in v15.01
    /// </remarks>
    public static PageFeature CmsWysiwyg = new("Cms.Wysiwyg", "Wysiwyg helpers / css for better rich content", urlWip: "dist/cms/wysiwyg.min.css");

}