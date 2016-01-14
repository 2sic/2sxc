using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Razor.Helpers;
using ToSic.Eav;

namespace ToSic.SexyContent.Adam
{
    public class Core
    {
        #region constants
        public const string AdamRootFolder = "adam/";
        public const string AdamAppRootFolder = "adam/[AppFolder]/";
        public const string AdamFolderMask = "adam/[AppFolder]/[Guid22]/[FieldName]/[SubFolder]";
        #endregion

        private SexyContent Sexy;
        private App App;
        private Razor.Helpers.DnnHelper Dnn;
        private readonly Guid entityGuid;
        private readonly string fieldName;
        private IFolderManager folderManager = FolderManager.Instance;
    

        public Core(SexyContent sexy, App app, Razor.Helpers.DnnHelper dnn, Guid eGuid, string fName)
        {
            Sexy = sexy;
            App = app;
            Dnn = dnn;
            entityGuid = eGuid;
            fieldName = fName;
        }



        private IFolderInfo _folder;

        /// <summary>
        /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
        /// Will create the folder if it does not exist
        /// </summary>
        internal IFolderInfo Folder(string subFolder, bool autoCreate)
        {
            IFolderInfo fldr;

            var path = GeneratePath(subFolder);

            // create all folders to ensure they exist. Must do one-by-one because dnn must have it in the catalog
            var pathParts = path.Split('/');
            var pathToCheck = ""; // pathParts[0];
            foreach (string part in pathParts.Where(p => !String.IsNullOrEmpty(p)))
            {
                pathToCheck += part + "/";
                if (Exists(pathToCheck)) continue;
                if (autoCreate)
                    Add(pathToCheck);
                else
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "subfolder " + pathToCheck + "not found" });
            }

            fldr = Get(path);

            return fldr;
        }

        internal bool Exists(string path)
        {
            return folderManager.FolderExists(Dnn.Portal.PortalId, path);
        }

        internal  IFolderInfo Add(string path)
        {
            return folderManager.AddFolder(Dnn.Portal.PortalId, path);
        }

        internal IFolderInfo Get(string path)
        {
            return folderManager.GetFolder(Dnn.Portal.PortalId, path);
        }

        public string GeneratePath(string subFolder)
        {
            var path = AdamFolderMask
                .Replace("[AppFolder]", App.Folder)
                .Replace("[Guid22]", GuidHelpers.Compress22(entityGuid))
                .Replace("[FieldName]", fieldName)
                .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
                .Replace("//", "/"); // sometimes has duplicate slashes if subfolder blank but sub-sub is given
            return path;
        }

        public string GenerateWebPath(AdamFile currentFile)
        {
            return Dnn.Portal.HomeDirectory + currentFile.Folder + currentFile.FileName;
        }

        public string GenerateWebPath(AdamFolder currentFolder)
        {
            return Dnn.Portal.HomeDirectory + currentFolder.FolderPath;
        }

        internal IFolderInfo Folder()
        {
            if (_folder == null)
                _folder = Folder("", true);
            return _folder;
        }

        public int GetMetadataId(int id, bool isFolder)
        {
            var items = GetFirstMetadataEntity(id, isFolder);
            //App.Data.Metadata.GetAssignedEntities(Constants.AssignmentObjectTypeCmsObject,
            //    (isFolder ? "folder:" : "file:") + id);

            return items?.EntityId ?? 0;
        }

        public IEntity GetFirstMetadataEntity(int id, bool isFolder)
        {
            return App.Data.Metadata.GetAssignedEntities(Constants.AssignmentObjectTypeCmsObject, 
                (isFolder ? "folder:" : "file:") + id)
                .FirstOrDefault();
        }
        public DynamicEntity GetFirstMetadata(int id, bool isFolder)
        {
            var meta = GetFirstMetadataEntity(id, isFolder);

            if (meta == null)
            {
                var emptyMetadata = new Dictionary<string, object>();
                emptyMetadata.Add("Title", "");
                meta = new Eav.Data.Entity(0, "", emptyMetadata, "Title");
            }
            return new DynamicEntity(meta, new[] {Thread.CurrentThread.CurrentCulture.Name}, Sexy);


        }

        #region type information
        internal string TypeName(string ext)
        {
            switch (ext.ToLower())
            {
                case "png":
                case "jpg":
                case "jpgx":
                case "jpeg":
                case "gif":
                    return "image";
                case "doc":
                case "docx":
                case "txt":
                case "pdf":
                case "xls":
                case "xlsx":
                    return "document";
                default:
                    return "file";
            }
        }
        #endregion

    }
}