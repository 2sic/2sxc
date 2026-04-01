using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using ToSic.Sys.Users;

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
            var result = new SxcModuleActionsBuilder(GetService<IUser>())
                .Build(ModuleConfiguration, Block, ModuleId, GetNextActionID, LocalizeString);
            return l.Return(result);
        }
        catch (Exception e)
        {
            Exceptions.LogException(e);
            return l.ReturnAsError([]);
        }
    }

    #endregion
}
