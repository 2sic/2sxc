using System;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn
{
    public partial class View
    {
        #region ModuleActions on THIS DNN-Module

        /// <summary>
        /// Causes DNN to create the menu with all actions like edit entity, new, etc.
        /// </summary>
        private ModuleActionCollection _moduleActions;
        public ModuleActionCollection ModuleActions
        {
            get
            {
                try
                {
                    if (_moduleActions != null) return _moduleActions;

                    // Don't offer options if it's from another portal
                    if (ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID)
                        _moduleActions = new ModuleActionCollection();

                    return _moduleActions = InitModuleActions();
                }
                catch (Exception e)
                {
                    Exceptions.LogException(e);
                    return new ModuleActionCollection();
                }
            }
        }

        private ModuleActionCollection InitModuleActions()
        {
            var actions = new ModuleActionCollection();
            if (Block == null) return actions;
            var block = Block;
            var appIsKnown = block.AppId > 0;
            if (appIsKnown)
            {
                // Edit item
                if (!block.View?.UseForList ?? false)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").edit();", "test", true,
                        SecurityAccessLevel.Edit, true, false);

                // Change layout button
                actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif",
                    "javascript:$2sxcActionMenuMapper(" + ModuleId + ").changeLayoutOrContent();", false,
                    SecurityAccessLevel.Edit, true, false);
            }

            var user = new DnnUser();

            // Edit Template Button
            if (user.IsDesigner && appIsKnown && block.View != null)
                actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent,
                    "templatehelp", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").develop();", "test",
                    true,
                    SecurityAccessLevel.Edit, true, false);

            // App management
            if (user.IsAdmin && appIsKnown)
                actions.Add(GetNextActionID(), "Admin" + (block.IsContentApp ? "" : " " + block.App?.Name), "",
                    "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                    SecurityAccessLevel.Admin, true, false);

            // Zone management (app list)
            if (user.IsAdmin)
                actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif",
                    "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminZone();", "", true,
                    SecurityAccessLevel.Admin, true, false);
            
            return actions;
        }

        #endregion

    }
}