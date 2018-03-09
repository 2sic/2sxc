using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.Identity;

namespace ToSic.SexyContent.Adam
{
    public class AdamBrowseContext
    {
        #region constants
        private const string AdamFolderMask = "[AdamRoot]/[Guid22]/[FieldName]/[SubFolder]";
        #endregion

        private readonly SxcInstance _sxcInstance;
        private readonly App _app;
        public readonly AdamManager AdamManager;
        private readonly ITennant _tennant;
        private readonly Guid _entityGuid;
        private readonly string _fieldName;
        private readonly bool _usePortalRoot;

        public AdamBrowseContext(SxcInstance sxcInstance, App app, ITennant tennant, Guid eGuid, string fName, bool usePortalRoot)
        {
            _tennant = tennant;
            AdamManager = new AdamManager(tennant.Id, app);
            _sxcInstance = sxcInstance;
            _app = app;
            _entityGuid = eGuid;
            _fieldName = fName;
            _usePortalRoot = usePortalRoot;
        }



        private Folder _folder;

        /// <summary>
        /// Get the folder specified in App.Settings (BasePath) combined with the module's ID
        /// Will create the folder if it does not exist
        /// </summary>
        internal Folder Folder(string subFolder, bool autoCreate)
        {
            var path = GeneratePath(subFolder);
            return AdamManager.Folder(path, autoCreate);
        }

        public string EntityRoot => GeneratePath("");

        public string GeneratePath(string subFolder)
        {
            // Enable portal browsing if requested
            if (_usePortalRoot)
                return (subFolder ?? "").Replace("//", "/");
            var path = AdamFolderMask
                .Replace("[AdamRoot]", AdamManager.RootPath)
                //.Replace("[AppFolder]", App.Folder)
                .Replace("[Guid22]", Mapper.GuidCompress(_entityGuid))
                .Replace("[FieldName]", _fieldName)
                .Replace("[SubFolder]", subFolder) // often blank, so it will just be removed
                .Replace("//", "/"); // sometimes has duplicate slashes if subfolder blank but sub-sub is given
            return path;
        }

        public string GenerateWebPath(AdamFile currentFile) 
            => _tennant.ContentPath + currentFile.Folder + currentFile.FileName;

        public string GenerateWebPath(AdamFolder currentFolder) 
            => _tennant.ContentPath + currentFolder.FolderPath;

        internal Folder Folder() => _folder ?? (_folder = Folder("", true));

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
                var emptyMetadata = new Dictionary<string, object> {{"Title", ""}};
                meta = new Eav.Data.Entity(Eav.Constants.TransientAppId, 0, ContentTypeBuilder.Fake(""), emptyMetadata, "Title");
            }
            return new DynamicEntity(meta, new[] {Thread.CurrentThread.CurrentCulture.Name}, _sxcInstance);


        }

        #region type information
        #endregion


    }
}