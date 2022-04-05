using System.Collections.Generic;
using ToSic.Eav.Configuration;
using static ToSic.Eav.Configuration.FeaturesBuiltIn;

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
        public static PageFeature PageContext = new PageFeature("2sxc.ContextPage", "the $2sxc headers in the page so everything works");

        /// <summary>
        /// Internal feature, not published ATM
        /// </summary>
        public static PageFeature ModuleContext = new PageFeature("2sxc.ContextModule", "the $2sxc headers in the module tag");

        /// <summary>
        /// The core 2sxc JS libraries
        /// </summary>
        /// <remarks>
        /// Published the key '2sxc.JsCore' in v13.00, do not change
        /// </remarks>
        public static PageFeature JsCore = new PageFeature("2sxc.JsCore", "2sxc core js APIs", needs: new[]
        {
            PageContext.Key
        });

        /// <summary>
        /// WIP - this will probably be moved to local only in future, ATM it's global though
        /// </summary>
        public static PageFeature ToolbarsAuto = new PageFeature("2sxc.ToolbarsAuto", "Ensure that the toolbars automatically appear", needs: new[]
        {
            PageContext.Key
        });

        /// <summary>
        /// The 2sxc JS libraries for cms / edit actions
        /// </summary>
        /// <remarks>
        /// Published the key '2sxc.JsCms' in v13.00, do not change
        /// </remarks>
        public static PageFeature JsCms = new PageFeature("2sxc.JsCms", "2sxc inpage editing APIs", needs: new[]
            {
                JsCore.Key,
                ModuleContext.Key,
            },
            reqConditions: new List<Condition> { PublicEditForm.Condition });

        /// <summary>
        /// The 2sxc JS libraries for cms / edit actions
        /// </summary>
        /// <remarks>
        /// Published the key '2sxc.Toolbars' in v13.00, do not change
        /// </remarks>
        public static PageFeature Toolbars =
            new PageFeature("2sxc.Toolbars", "2sxc InPage editing UIs / Toolbar", needs: new[]
            {
                JsCore.Key,
                ToolbarsAuto.Key,
                JsCms.Key
            });

        /// <summary>
        /// turnOn feature
        /// </summary>
        /// <remarks>
        /// Published the key 'turnOn' in v12.02, do not change
        /// </remarks>
        public static PageFeature TurnOn = new PageFeature("turnOn", "turnOn JS library");

    }
}
