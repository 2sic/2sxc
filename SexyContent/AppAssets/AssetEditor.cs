using System;
using System.IO;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// This tells us if the file is in the apps global area (in portal _default) or in the local area (current portal)
        /// </summary>
        //public bool Global { get; }

        public AssetEditor(SxcInstance sexy, int templateId, UserInfo userInfo, PortalSettings portalSettings)//, bool global = false)
        {
            _sexy = sexy;
            _userInfo = userInfo;
            _portalSettings = portalSettings;
            //Global = global;

            var template = _sexy.AppTemplates.GetTemplate(templateId);
            EditInfo = TemplateAssetsInfo(template);
        }

        public AssetEditor(SxcInstance sexy, string path, UserInfo userInfo, PortalSettings portalSettings, bool global = false)
        {
            _sexy = sexy;
            _userInfo = userInfo;
            _portalSettings = portalSettings;
            //Global = global;

            EditInfo = new AssetEditInfo(_sexy.App.AppId, _sexy.App.Name, path, global);
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
        public void EnsureUserMayEditAsset(string fullPath = null)
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

            // optionally check if the file is really in the poprtal
            if (fullPath == null) return;

            var path = new FileInfo(fullPath);
            if(path.Directory == null)
                throw new AccessViolationException("path is null");

            if (path.Directory.FullName.IndexOf(_sexy.App.PhysicalPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                throw new AccessViolationException("current user may not edit files outside of the app-scope");
        }

        private AssetEditInfo TemplateAssetsInfo(Template templ)
        {
            var t = new AssetEditInfo(_sexy.App.AppId,_sexy.App.Name, templ.Path, templ.Location == Settings.TemplateLocations.HostFileSystem)
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
                EnsureUserMayEditAsset(InternalPath);
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
                EnsureUserMayEditAsset(InternalPath);

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

        public bool Create(string contents)
        {
            // todo: maybe add some security for special dangerous file names like .cs, etc.?
            EditInfo.FileName = Regex.Replace(EditInfo.FileName, @"[?:\/*""<>|]", "");
            var absolutePath = InternalPath;// server.MapPath(Path.Combine(GetTemplatePathRoot(location, App), templatePath));

            // don't create if it already exits
            if (File.Exists(absolutePath)) return false;

            // check if the folder to it already exists, or create it...
            var foundFolder = absolutePath.LastIndexOf("\\", StringComparison.InvariantCulture);
            if (foundFolder > -1)
            {
                var folderPath = absolutePath.Substring(0, foundFolder);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
            }

            // now create the file
            var stream = new StreamWriter(File.Create(absolutePath));
            stream.Write(contents);
            stream.Flush();
            stream.Close();

            return true;
        }
    }
}