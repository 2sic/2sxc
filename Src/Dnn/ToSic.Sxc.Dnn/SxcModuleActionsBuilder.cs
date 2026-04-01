using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sys.Users;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Dnn;

internal sealed class SxcModuleActionsBuilder(IUser user)
{
    public ModuleActionCollection Build(
        ModuleInfo moduleConfiguration,
        IBlock block,
        int moduleId,
        Func<int> nextActionId,
        Func<string, string> localize)
    {
        var actions = new ModuleActionCollection();
        if (moduleConfiguration == null || block == null || moduleConfiguration.PortalID != moduleConfiguration.OwnerPortalID)
            return actions;

        var appIsKnown = block.AppId > 0;
        var viewToUse = block.ViewIsReady ? block.View : null;
        if (appIsKnown)
        {
            if (viewToUse?.UseForList == true)
                actions.Add(nextActionId(), localize("ActionEdit.Text"), "", "", "edit.gif",
                    JsAction(moduleId, "edit", "{ useModuleList: true, index: 0 }"),
                    "test", true,
                    SecurityAccessLevel.Edit, true, false);

            actions.Add(nextActionId(), localize("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif",
                JsAction(moduleId, "layout"),
                false,
                SecurityAccessLevel.Edit, true, false);
        }

        if (user.IsSiteDeveloper && appIsKnown && viewToUse != null)
            actions.Add(nextActionId(), localize("ActionEditTemplateFile.Text"), ModuleActionType.EditContent,
                "templatehelp", "edit.gif",
                JsAction(moduleId, "template-develop"),
                "test",
                true,
                SecurityAccessLevel.Edit, true, false);

        if (user.IsSiteAdmin && appIsKnown)
            actions.Add(nextActionId(), "Admin" + (block.IsContentApp ? "" : " " + block.AppOrNull?.Name), "",
                "", "edit.gif",
                JsAction(moduleId, "app"),
                "", true,
                SecurityAccessLevel.Admin, true, false);

        if (user.IsSiteAdmin)
            actions.Add(nextActionId(), "Apps Management", "AppManagement.Action", "", "action_settings.gif",
                JsAction(moduleId, "zone"),
                "", true,
                SecurityAccessLevel.Admin, true, false);

        return actions;
    }

    private static string JsAction(int moduleId, string action, string commandParams = null)
    {
        var useParams = commandParams.HasValue() ? ", params: " + commandParams : "";
        return "javascript:$2sxc(" + moduleId + ").cms.run({ action: '" + action + "' " + useParams + " });";
    }
}
