using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7.Installation
{
    public class InstallationEnvironment: IInstallerEnvironment
    {
        public void UpgradeCompleted()
        {
            ClientResourceManager.UpdateVersion();
        }
    }
}
