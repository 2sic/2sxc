using DotNetNuke.Framework;
using ToSic.Eav.Documentation;

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
            // var ServiceProvider = HttpContext.Current.GetScope().ServiceProvider;
            return DnnStaticDi.StaticBuild<T>();
        }

    }
}
