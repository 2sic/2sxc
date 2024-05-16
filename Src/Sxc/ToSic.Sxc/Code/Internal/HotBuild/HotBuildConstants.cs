using ToSic.Eav;

namespace ToSic.Sxc.Code.Internal.HotBuild;

internal class HotBuildConstants
{
    /// <summary>
    /// Check if an object is from the AppCode-xxx.dll
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    internal static bool ObjectIsFromAppCode(object obj)
    {
        if (obj == null) return false;
        var ownType = obj.GetType();
        return (ownType.Namespace ?? "").StartsWith(Constants.AppCode)
               || ownType.Assembly.FullName.Contains(Constants.AppCode);
    }
}