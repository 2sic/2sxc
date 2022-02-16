using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;


namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// Permissions object which checks if the user is allowed to do something based on specific permission
    /// This checks permissions based on EAV data related to an entity - so pure EAV, no DNN
    /// </summary>
    public class DnnPermissionCheck: AppPermissionCheck
    {
        #region Constructor / DI

        public DnnPermissionCheck(IAppStates appStates, Dependencies dependencies) : base(appStates, dependencies, DnnConstants.LogName)
        {
        }

        #endregion
    }
}