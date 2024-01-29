using ToSic.Sxc.Backend.Admin;
using RealController = ToSic.Sxc.Backend.Admin.DialogControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
[SupportedModules(DnnSupportedModuleNames)]
[DnnLogExceptions]
// 2dm 2024-01-26 changed from "Admin" to "Edit" because it's used in the quick-dialog.
// It needs "Edit" to get settings for the quick-dialog-add-inner content
// https://github.com/2sic/2sxc/issues/3234
// If we refactor the quick-dialog it should provide the settings directly in the main call
// And not need this any more - then we can switch back to "Admin"
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DialogController() : DnnSxcControllerBase(RealController.LogSuffix), IDialogController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    public DialogContextStandaloneDto Settings(int appId) => Real.Settings(appId);
}