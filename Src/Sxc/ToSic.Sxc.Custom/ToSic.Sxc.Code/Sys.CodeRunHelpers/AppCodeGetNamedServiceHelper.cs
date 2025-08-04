using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Special helper for GetServiceByName finding a service in AppCode
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppCodeGetNamedServiceHelper
{
    public static TService GetService<TService>(object owner, CompileCodeHelperSpecs helperSpecs, string? typeName = default) where TService : class
    {
        // Standard case - just a normal GetService<T>()
        if (typeName.IsEmptyOrWs())
            return helperSpecs.ExCtx.GetService<TService>();

        var log = helperSpecs.ExCtx.Log;

        var ownType = owner.GetType();
        var assembly = ownType.Assembly;
        // Note: don't check the Namespace property, as it may be empty
        if (!HotBuildConstants.ObjectIsFromAppCode(owner))
            throw log.Ex(new Exception($"Type '{ownType.FullName}' is not in the 'AppCode' namespace / dll, so it can't be used to find other types."));

        var type = assembly.FindControllerTypeByName(typeName);

        return type == null
            ? throw log.Ex(new Exception($"Type '{typeName}' not found in assembly '{assembly.FullName}'"))
            : helperSpecs.ExCtx.GetService<TService>(type: type);
    }

}