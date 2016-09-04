using System;
using System.IO;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace ToSic.SexyContent.AppAssets
{
    internal class AssetEditor
    {
        public AssetEditInfo EditInfo { get; }

        private readonly UserInfo _userInfo;

        private readonly PortalSettings _portalSettings;

        private readonly SxcInstance _sexy;

        public AssetEditor(SxcInstance sexy, int templateId, UserInfo userInfo, PortalSettings portalSettings)
        {
            _sexy = sexy;
            _userInfo = userInfo;
            _portalSettings = portalSettings;

            var template = _sexy.AppTemplates.GetTemplate(templateId);
            EditInfo = TemplateAssetsInfo(template);
        }

        public AssetEditor(SxcInstance sexy, string path, UserInfo userInfo, PortalSettings portalSettings)
        {
            _sexy = sexy;
            _userInfo = userInfo;
            _portalSettings = portalSettings;

            EditInfo = new AssetEditInfo(_sexy.App.AppId, _sexy.App.Name, path);
        }

        public AssetEditInfo EditInfoWithSource

        {
            get
            {
                EditInfo.Code = Source; // do this later, because it relies on the edit-info to exist
                return EditInfo;
            }
        }

        /// <summary>
        /// Check permissions and if not successful, give detailed explanation
        /// </summary>
        public void EnsureUserMayEditAsset()
        {
            // check super user permissions - then all is allowed
            if (_userInfo.IsSuperUser)
                return;

            // ensure current user is admin - this is the minimum of not super-user
            if(!_userInfo.IsInRole(_portalSettings.AdministratorRoleName))
                throw new AccessViolationException("current user may not edit templates, requires admin rights");

            // if not super user, check if razor (not allowed; super user only)
            if(!EditInfo.IsSafe)
                throw new AccessViolationException("current user may not edit razor templates - requires super user");

            // if not super user, check if cross-portal storage (not allowed; super user only)
            if(EditInfo.LocationScope != Settings.TemplateLocations.PortalFileSystem)
                throw new AccessViolationException("current user may not edit templates in central storage - requires super user");
        }

        private AssetEditInfo TemplateAssetsInfo(Template templ)
        {
            var t = new AssetEditInfo(_sexy.App.AppId,_sexy.App.Name, templ.Path)
            {
                // Template specific properties, not really available in other files
                LocationScope = templ.Location,
                Type = templ.Type,
                Name = templ.Name,
                HasList = templ.UseForList,
                TypeContent = templ.ContentTypeStaticName,
                TypeContentPresentation = templ.PresentationTypeStaticName,
                TypeList = templ.ListContentTypeStaticName,
                TypeListPresentation = templ.ListPresentationTypeStaticName
            };
            return t;
        }

        public string InternalPath => HttpContext.Current.Server.MapPath(
            Path.Combine(
                Internal.TemplateManager.GetTemplatePathRoot(EditInfo.LocationScope, _sexy.App),
                EditInfo.FileName));


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

                throw new FileNotFoundException("could not find file" 
                    + (_userInfo.IsSuperUser 
                    ? " for superuser - file tried '" + InternalPath + "'" 
                    : "")
                    );
            }
            set
            {
                EnsureUserMayEditAsset();

                if (File.Exists(InternalPath))
                    File.WriteAllText(InternalPath, value);
                else
                    throw new FileNotFoundException("could not find file"
                        + (_userInfo.IsSuperUser
                        ? " for superuser - file tried '" + InternalPath + "'"
                        : "")
                        );

            }
        }

    }
}