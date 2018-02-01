using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class EntityBase
    {
        #region constants
        //public const string AdamRootFolder = "adam/";
        private const string AdamFolderMask = "[AdamRoot]/[Guid22]/[FieldName]/[SubFolder]";
        #endregion

        private readonly SxcInstance _sxcInstance;
        private readonly App _app;
        private readonly AdamManager _adamManager;
        private readonly PortalSettings _portalSettings;
        private readonly Guid _entityGuid;
        private readonly string _fieldName;
        private readonly bool _usePortalRoot;

        public EntityBase(SxcInstance sxcInstance, App app, PortalSettings ps, Guid eGuid, string fName, bool usePortalRoot)
        {
            _portalSettings = ps;
            _adamManager = new AdamManager(ps.PortalId, app);
            _sxcInstance = sxcInstance;
            _app = app;
            _entityGuid = eGuid;
            _fieldName = fName;
            _usePortalRoot = usePortalRoot;
        }



        private IFolderInfo _folder;

        /// <summary>
        /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
        /// Will create the folder if it does not exist
        /// </summary>
        internal IFolderInfo Folder(string subFolder, bool autoCreate)
        {
            var path = GeneratePath(subFolder);
            return _adamManager.Folder(path, autoCreate);
        }

        public string EntityRoot => GeneratePath("");

        public string GeneratePath(string subFolder)
        {
            // Enable portal browsing if requested
            if (_usePortalRoot)
                return (subFolder ?? "").Replace("//", "/");
            var path = AdamFolderMask
                .Replace("[AdamRoot]", _adamManager.RootPath)
                //.Replace("[AppFolder]", App.Folder)
                .Replace("[Guid22]", GuidHelpers.Compress22(_entityGuid))
                .Replace("[FieldName]", _fieldName)
                .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
                .Replace("//", "/"); // sometimes has duplicate slashes if subfolder blank but sub-sub is given
            return path;
        }

        public string GenerateWebPath(AdamFile currentFile) 
            => _portalSettings.HomeDirectory + currentFile.Folder + currentFile.FileName;

        public string GenerateWebPath(AdamFolder currentFolder) 
            => _portalSettings.HomeDirectory + currentFolder.FolderPath;

        internal IFolderInfo Folder() => _folder ?? (_folder = Folder("", true));

        public int GetMetadataId(int id, bool isFolder)
        {
            var items = GetFirstMetadataEntity(id, isFolder);

            return items?.EntityId ?? 0;
        }

        public Eav.Interfaces.IEntity GetFirstMetadataEntity(int id, bool isFolder) 
            => _app.Data.Metadata.GetMetadata(Eav.Constants.MetadataForCmsObject, 
                (isFolder ? "folder:" : "file:") + id)
            .FirstOrDefault();

        public DynamicEntity GetFirstMetadata(int id, bool isFolder)
        {
            var meta = GetFirstMetadataEntity(id, isFolder);

            if (meta == null)
            {
                var emptyMetadata = new Dictionary<string, object>();
                emptyMetadata.Add("Title", "");
                meta = new Eav.Data.Entity(Eav.Constants.TransientAppId, 0, "", emptyMetadata, "Title");
            }
            return new DynamicEntity(meta, new[] {Thread.CurrentThread.CurrentCulture.Name}, _sxcInstance);


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