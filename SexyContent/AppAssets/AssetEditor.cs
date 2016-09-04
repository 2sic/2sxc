using System;
using System.IO;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace ToSic.SexyContent.AppAssets
{
    internal class AssetEditor
    {
        private Template Template { get; set;}

        private readonly UserInfo _userInfo;

        private readonly PortalSettings _portalSettings;

        private readonly SxcInstance _sexy;

        public AssetEditor(SxcInstance sexy, int templateId, UserInfo userInfo, PortalSettings portalSettings)
        {
            _sexy = sexy;
            Template = _sexy.AppTemplates.GetTemplate(templateId);
            _userInfo = userInfo;
            _portalSettings = portalSettings;
        }
        

        /// <summary>
        /// Check permissions and if not successful, give detailed explanation
        /// </summary>
        public void EnsureUserMayEditAsset()
        {
            // check super user permissions
            if (_userInfo.IsSuperUser)
                return;

            // ensure current user is admin
            if(!_userInfo.IsInRole(_portalSettings.AdministratorRoleName))
                throw new AccessViolationException("current user may not edit templates, requires admin rights");

            // if not super user, check if razor (not allowed)
            if(Template.IsRazor)
                throw new AccessViolationException("current user may not edit razor templates - requires super user");

            // if not super user, check if cross-portal storage (not allowed)
            if(Template.Location == Settings.TemplateLocations.PortalFileSystem)
                throw new AccessViolationException("current user may not edit templates in central storage - requires super user");
        }

        public AssetEditInfo EditInfo()
        {
            var templ = Template;

            var t = new AssetEditInfo
            {
                Type = templ.Type,
                FileName = templ.Path,
                Code = Source,
                Name = templ.Name,
                HasList = templ.UseForList,
                HasApp = _sexy.App.Name != "Content",
                AppId = _sexy.App.AppId,
                TypeContent = templ.ContentTypeStaticName,
                TypeContentPresentation = templ.PresentationTypeStaticName,
                TypeList = templ.ListContentTypeStaticName,
                TypeListPresentation = templ.ListPresentationTypeStaticName
            };
            return t;
        }

        public string InternalPath => HttpContext.Current.Server.MapPath(
            Path.Combine(
                Internal.TemplateManager.GetTemplatePathRoot(Template.Location, _sexy.App),
                Template.Path));


        /// <summary>
        /// Read / Write the source code of the template file
        /// </summary>
        public string Source
        {
            get
            {
                EnsureUserMayEditAsset();
                if (File.Exists(InternalPath))
                    return File.ReadAllText(InternalPath);

                throw new FileNotFoundException("could not find template file");
            }
            set
            {
                EnsureUserMayEditAsset();

                if (File.Exists(InternalPath))
                    File.WriteAllText(InternalPath, value);
                else
                    throw new FileNotFoundException("could not find template file");

            }
        }

    }
}