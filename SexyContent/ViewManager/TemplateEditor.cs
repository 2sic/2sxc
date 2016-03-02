using System;
using System.IO;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace ToSic.SexyContent.ViewManager
{
    internal class TemplateEditor
    {
        public Template Template { get; internal set;}

        public UserInfo UserInfo;

        public PortalSettings PortalSettings;

        public SxcInstance Sexy;

        public TemplateEditor(SxcInstance sexy, int templateId, UserInfo userInfo, PortalSettings portalSettings)
        {
            Sexy = sexy;
            Template = Sexy.AppTemplates.GetTemplate(templateId);
            UserInfo = userInfo;
            PortalSettings = portalSettings;
        }
        

        /// <summary>
        /// Check permissions and if not successful, give detailed explanation
        /// </summary>
        public void EnsureUserMayEditTemplate()
        {
            // check super user permissions
            if (UserInfo.IsSuperUser)
                return;

            // ensure current user is admin
            if(!UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                throw new AccessViolationException("current user may not edit templates, requires admin rights");

            // if not super user, check if razor (not allowed)
            if(Template.IsRazor)
                throw new AccessViolationException("current user may not edit razor templates - requires super user");

            // if not super user, check if cross-portal storage (not allowed)
            if(Template.Location == Settings.TemplateLocations.PortalFileSystem)
                throw new AccessViolationException("current user may not edit templates in central storage - requires super user");
        }


        public string InternalPath
        {
            get
            {
                return
                    HttpContext.Current.Server.MapPath(
                        Path.Combine(
                            Internal.TemplateManager.GetTemplatePathRoot(Template.Location, Sexy.App),
                            Template.Path));
            }
        }


        /// <summary>
        /// Read / Write the source code of the template file
        /// </summary>
        public string Code
        {
            get
            {
                EnsureUserMayEditTemplate();
                if (File.Exists(InternalPath))
                    return File.ReadAllText(InternalPath);

                throw new FileNotFoundException("could not find template file");
            }
            set
            {
                EnsureUserMayEditTemplate();

                if (File.Exists(InternalPath))
                    File.WriteAllText(InternalPath, value);
                else
                    throw new FileNotFoundException("could not find template file");

            }
        }

    }
}