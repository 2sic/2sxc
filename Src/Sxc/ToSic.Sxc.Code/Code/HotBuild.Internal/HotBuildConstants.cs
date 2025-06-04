using ToSic.Eav;
using ToSic.Eav.Sys;

namespace ToSic.Sxc.Code.Internal.HotBuild;

// TODO: MAKE class INTERNAL AGAIN AFTER MOVING TO ToSic.Sxc.Custom

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HotBuildConstants
{
    /// <summary>
    /// Check if an object is from the AppCode-xxx.dll
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool ObjectIsFromAppCode(object obj)
    {
        if (obj == null) return false;
        var ownType = obj.GetType();
        return (ownType.Namespace ?? "").StartsWith(FolderConstants.AppCode)
               || ownType.Assembly.FullName.Contains(FolderConstants.AppCode);
    }
}