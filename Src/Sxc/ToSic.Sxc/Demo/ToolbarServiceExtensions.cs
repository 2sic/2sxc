using ToSic.Sxc.Services;

namespace ToSic.Sxc.Demo
{
    /// <summary>
    /// Demo extensions to help in tutorials with the ToolbarService.
    ///
    /// Not meant for production, could change at any time. 
    /// </summary>
    public static class ToolbarServiceExtensions
    {
        /// <summary>
        /// Created for 14.07.05, used in the tutorial starting 2022-08-23
        /// </summary>
        /// <param name="toolbarService"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="ui"></param>
        public static void ActivateDemoMode(this IToolbarService toolbarService, 
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null
            )
        {
            if (!(toolbarService is ToolbarService typed)) return;
            typed._setDemoDefaults(ui);
        }
    }
}
