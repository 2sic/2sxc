using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Dnn;

partial class View
{
    #region ModuleActions on THIS DNN-Module

    /// <summary>
    /// Dnn will create the menu with all actions like edit entity, app settings, etc.
    /// </summary>
    public ModuleActionCollection ModuleActions => field ??= GetModuleActions();

    private ModuleActionCollection GetModuleActions()
    {
        var l = Log.Fn<ModuleActionCollection>();
        try
        {
            // Don't offer options if it's from another portal
            if (ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID)
                return l.Return([]);

            var result = InitModuleActions();
            return l.Return(result);
        }
        catch (Exception e)
        {
            Exceptions.LogException(e);
            return l.ReturnAsError([]);
        }

    }

    private ModuleActionCollection InitModuleActions()
    {
        var actions = new ModuleActionCollection();
        if (Block == null)
            return actions;
        var block = Block;
        var appIsKnown = block.AppId > 0;
        if (appIsKnown)
        {
            // Edit item
            if (!block.View?.UseForList ?? false)
                actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif",
                    JsAction("edit", "{ useModuleList: true, index: 0 }"),
                    "test", true,
                    SecurityAccessLevel.Edit, true, false);

            // Change layout button
            actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif",
                JsAction("layout"),
                false,
                SecurityAccessLevel.Edit, true, false);
        }

        var user = GetService<IUser>();

        // Edit Template Button
        if (user.IsSiteDeveloper && appIsKnown && block.View != null)
            actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent,
                "templatehelp", "edit.gif",
                JsAction("template-develop"),
                "test",
                true,
                SecurityAccessLevel.Edit, true, false);

        // App management
        if (user.IsSiteAdmin && appIsKnown)
            actions.Add(GetNextActionID(), "Admin" + (block.IsContentApp ? "" : " " + block.App?.Name), "",
                "", "edit.gif",
                JsAction("app"),
                "", true,
                SecurityAccessLevel.Admin, true, false);

        // Zone management (app list)
        if (user.IsSiteAdmin)
            actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif",
                JsAction("zone"),
                "", true,
                SecurityAccessLevel.Admin, true, false);
            
        return actions;
    }

    private string JsAction(string action, string commandParams = null)
    {
        var useParams = commandParams.HasValue() ? ", params: " + commandParams : "";
        return "javascript:$2sxc(" + ModuleId + ").cms.run({ action: '" + action + "' " + useParams + " });";
    }

    #endregion

}