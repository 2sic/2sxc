using System.Reflection;

namespace ToSic.Sxc.Code.Internal.HotBuild;

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
                   ?? appCodeAssembly.GetTypes().FirstOrDefault(t => t.Name == controllerTypeName);
        return type;
    }
}