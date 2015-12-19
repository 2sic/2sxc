using System.IO;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace ToSic.SexyContent.ViewManager
{
    public class ViewEditor
    {
        private Template _template;
        public Template Template
        {
            get
            {
                if (_template == null)
                  return Sexy.Templates.GetTemplate(templateId);
                return _template;
            }
        }

        private int templateId;

        public UserInfo UserInfo;

        public PortalSettings PortalSettings;

        public SexyContent Sexy;

        public ViewEditor(SexyContent sexy, int templId, UserInfo userInfo, PortalSettings portalSettings)
        {
            Sexy = sexy;
            templateId = templId;
            UserInfo = userInfo;
            PortalSettings = portalSettings;
        }

        /// <summary>
        /// Verify if the current user is allowed to edit this view
        /// </summary>
        public bool UserMayEditView
        {
            get
            {
                return UserInfo.IsSuperUser ||
                    (Template.Location == SexyContent.TemplateLocations.PortalFileSystem && !Template.IsRazor && UserInfo.IsInRole(PortalSettings.AdministratorRoleName));
            }
        }
        

    }
}