using ToSic.Eav;
using System.Reflection;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[PrivateApi("Internal stuff")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class AppCodeExtensions
{
    /// <summary>
    /// Find ApiController Type by name in AppCode Assembly
    /// </summary>
    /// <param name="appCodeAssembly"></param>
    /// <param name="controllerTypeName"></param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    public static Type FindControllerTypeByName(this Assembly appCodeAssembly, string controllerTypeName)
    {
        var type = appCodeAssembly.GetType(controllerTypeName, false, true)
                   // Find in case it's in a namespace
                   ?? appCodeAssembly.GetTypes().FirstOrDefault(t => t.Name.Equals(controllerTypeName, StringComparison.OrdinalIgnoreCase));
        return type;
    }

    // 2024-05-02 2dm seems unused, maybe old
    ///// <summary>
    ///// Return list of types that are controllers
    ///// </summary>
    ///// <param name="appCodeAssembly"></param>
    ///// <returns>list of types that are controllers</returns>
    //public static List<Type> GetAllControllerTypes(this Assembly appCodeAssembly) 
    //    => appCodeAssembly.GetTypes().Where(t => t.Name.EndsWith(Constants.ApiControllerSuffix, StringComparison.OrdinalIgnoreCase)).ToList();
}