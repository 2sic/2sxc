using System.Web;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Framework;
using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// Provides extension methods for DNN Skins (Themes) and Modules.
    /// </summary>
    /// <remarks>
    /// WIP v13.11+
    /// </remarks>
    [PrivateApi("WIP v14")]
    public static class UserControlBaseExtensions
    {
        public static T GetService<T>(this UserControlBase skinOrModule)
        {
            // Now DNN 9 only - this must work
            return HttpContext.Current.GetScope().ServiceProvider.Build<T>();
            //return DnnStaticDi.StaticBuild<T>();
        }

        public static T GetService<T>(this System.Web.UI.UserControl skinOrModule)
        {
            // Now DNN 9 only - this must work
            return HttpContext.Current.GetScope().ServiceProvider.Build<T>();
            //return DnnStaticDi.StaticBuild<T>();
        }

    }
}
