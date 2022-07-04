using System.Collections.Generic;
using ToSic.Eav.Configuration;
using static ToSic.Eav.Configuration.BuiltInFeatures;

namespace ToSic.Sxc.Web.PageFeatures
{
    public class BuiltInFeatures
    {
        /// <summary>
        /// JQuery feature
        /// </summary>
        /// <remarks>
        /// Published the key 'jQuery' in v12.02, do not change
        /// </remarks>
        public static PageFeature JQuery = new PageFeature("jQuery", "jQuery");
        
        /// <summary>
        /// Internal feature, not published ATM
        /// </summary>
        public static PageFeature ContextPage = new PageFeature("2sxc.ContextPage", "the $2sxc headers in the page so everything works");

        /// <summary>
        /// Internal feature, not published ATM
        /// </summary>
        public static PageFeature ContextModule = new PageFeature("2sxc.ContextModule", "the $2sxc headers in the module tag");

        /// <summary>
        /// The core 2sxc JS libraries
        /// </summary>
        /// <remarks>
        /// Published the key '2sxc.JsCore' in v13.00, do not change
        /// </remarks>
        public static PageFeature JsCore = new PageFeature("2sxc.JsCore", "2sxc core js APIs", needs: new[]
        {
            ContextPage.NameId
        });

        /// <summary>
        /// The INTERNAL USE 2sxc JS libraries for cms / edit actions.
        /// This one doesn't check requirements, and is the one which is added automatically. 
        /// </summary>
        public static PageFeature JsCmsInternal = new PageFeature("Internal.JsCms", "2sxc inpage editing APIs - internal version without checks", needs: new[]
            {
                JsCore.NameId,
                ContextModule.NameId,
            });

        private static readonly List<Condition> RequiresPublicEditForm = new List<Condition> { PublicEditForm.Condition };

        /// <summary>
        /// The 2sxc JS libraries for cms / edit actions
        /// </summary>
        /// <remarks>
        /// Published the key '2sxc.JsCms' in v13.00, do not change
        /// </remarks>
        public static PageFeature JsCms = new PageFeature("2sxc.JsCms", 
            "2sxc inpage editing APIs", 
            needs: new[] { JsCmsInternal.NameId },
            requirements: RequiresPublicEditForm);

        /// <summary>
        /// The 2sxc JS libraries for cms / edit actions
        /// </summary>
        /// <remarks>
        /// Published the key '2sxc.Toolbars' in v13.00, do not change
        /// </remarks>
        public static PageFeature ToolbarsInternal = new PageFeature("Internal.Toolbars",
            "2sxc InPage editing UIs / Toolbar",
            needs: new[]
            {
                JsCmsInternal.NameId,
                ContextPage.NameId,
            });

        /// <summary>
        /// The 2sxc JS libraries for cms / edit actions
        /// </summary>
        /// <remarks>
        /// Published the key '2sxc.Toolbars' in v13.00, do not change
        /// </remarks>
        public static PageFeature Toolbars = new PageFeature("2sxc.Toolbars",
            "2sxc InPage editing UIs / Toolbar",
            needs: new[] { ToolbarsInternal.NameId },
            requirements: RequiresPublicEditForm);

        /// <summary>
        /// WIP - this will probably be moved to local only in future, ATM it's global though
        /// </summary>
        public static PageFeature ToolbarsAutoInternal = new PageFeature("Internal.ToolbarsAuto",
            "Ensure that the toolbars automatically appear", needs: new[]
            {
                ContextPage.NameId,
                ToolbarsInternal.NameId,
            });

        /// <summary>
        /// WIP - this will probably be moved to local only in future, ATM it's global though
        /// </summary>
        public static PageFeature ToolbarsAuto = new PageFeature("2sxc.ToolbarsAuto",
            "Ensure that the toolbars automatically appear",
            needs: new[] { ToolbarsAutoInternal.NameId },
            requirements: RequiresPublicEditForm);

        /// <summary>
        /// turnOn feature
        /// </summary>
        /// <remarks>
        /// Published the key 'turnOn' in v12.02, do not change
        /// </remarks>
        public static PageFeature TurnOn = new PageFeature("turnOn", "turnOn JS library");

    }
}
