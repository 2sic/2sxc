using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using ToSic.Eav.Environment.Sys.Permissions;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnEnvironmentPermission() : EnvironmentPermission(DnnConstants.LogName)
{
    public string CustomPermissionKey = ""; // "CONTENT";

    /// <summary>
    /// The DNN module on the container
    /// </summary>
    /// <remarks>
    /// In some cases the container is a ContainerNull object without ModuleInfo, so we must really do null-checks
    /// </remarks>
    protected ModuleInfo Module => _module.Get(
        () => ((Context as IContextOfBlock)?.Module as Module<ModuleInfo>)?.GetContents());
    private readonly GetOnce<ModuleInfo> _module = new();

    public override bool VerifyConditionOfEnvironment(string condition)
    {
        var l = Log.Fn<bool>($"condition: {condition}");
        var fullPrefix = (SalPrefix + ".").ToLowerInvariant();
        if (!condition.StartsWith(fullPrefix, StringComparison.InvariantCultureIgnoreCase))
            return l.ReturnFalse("unknown condition: false");

        var salWord = condition.Substring(fullPrefix.Length);
        var sal = (SecurityAccessLevel)Enum.Parse(typeof(SecurityAccessLevel), salWord);
        // check anonymous - this is always valid, even if not in a module context
        if (sal == SecurityAccessLevel.Anonymous)
            return l.ReturnTrue("anonymous, always true");

        // check within module context
        if (Module != null)
        {
            // TODO: STV WHERE DOES THE MODULE COME FROM?
            // IT APPEARS THAT IT'S MISSING IN NORMAL REST CALLS
            var result = ModulePermissionController.HasModuleAccess(sal, CustomPermissionKey, Module);
            return l.Return(result, $"module: {result}");
        }

        l.A("trying to check permission " + fullPrefix + ", but don't have module in context");
        return l.ReturnFalse("can't verify: false");
    }

    protected override bool UserIsModuleAdmin()
    {
        var l = Log.Fn<bool>();
        return l.ReturnAsOk(Module != null && ModulePermissionController.CanAdminModule(Module));
    }
 

    protected override bool UserIsModuleEditor()
    {
        var l = Log.Fn<bool>();
        if (Module == null)
            return false;

        // This seems to throw errors during search :(
        try
        {
            // skip during search (usual HttpContext is missing for search)
            if (System.Web.HttpContext.Current == null)
                return l.ReturnFalse();

            var isEditor = ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "", Module)
                           || ModulePermissionController.CanEditModuleContent(Module);
            return l.ReturnAndLog(isEditor);
        }
        catch (Exception e)
        {
            l.Ex(e);
            return l.ReturnFalse("error");
        }
    }
}