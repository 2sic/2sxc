using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;

// Important Info to people working with this
// It depends on abstract provder, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of App files and folders from the current platform (Dnn or Oqtane).
    ///
    /// As of now there are no parameters to set.
    ///
    /// To figure out the properties returned and what they match up to, see <see cref="PageDataRaw"/>
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = "", // TODO:
        GlobalName = "3fe6c215-4c37-45c1-8883-b4b2a47162a7",
        HelpLink = "https://r.2sxc.org/ds-appfiles",
        Icon = Icons.Tree,
        NiceName = "AppFiles",
        Type = DataSourceType.Source,
        Audience = Audience.Advanced,
        UiHint = "Files and folders in the App folder")]
    public class AppFiles: ExternalData
    {
        private readonly IDataBuilder _folderBuilder;
        private readonly ITreeMapper _treeMapper;
        private readonly IDataBuilder _fileBuilder;
        private readonly AppFilesDataSourceProvider _provider;

        #region Configuration properties

        [Configuration(Fallback = false)]
        public bool OnlyFolders
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }

        [Configuration(Fallback = false)]
        public bool OnlyFiles
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }

        [Configuration(Fallback = "/")]
        public string Root
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        [Configuration(Fallback = "*.*")]
        public string Filter
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        #endregion

        #region Constructor

        [PrivateApi]
        public AppFiles(MyServices services, AppFilesDataSourceProvider provider, IDataBuilder fileDataBuilder, IDataBuilder folderBuilder, ITreeMapper treeMapper) : base(services, "CDS.AppFiles")
        {
            ConnectServices(
                _provider = provider,
                _fileBuilder = fileDataBuilder,
                _folderBuilder = folderBuilder,
                _treeMapper = treeMapper
            );

            Provide(GetDefault);
            Provide("Folders", GetFolders);
            Provide("Files", GetFiles);
        }
        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetDefault() => GetInternal().Where(e => e.Type.Name == AppFileDataRaw.TypeName).ToImmutableList();

        [PrivateApi]
        public IImmutableList<IEntity> GetFolders() => GetInternal().Where(e => e.Type.Name == AppFolderDataRaw.TypeName).ToImmutableList();

        [PrivateApi]
        public IImmutableList<IEntity> GetFiles() => GetInternal();

        private IImmutableList<IEntity> GetInternal() => _getInternal.Get(() => Log.Func(l =>
        {
            Configuration.Parse();

            _provider.Configure(zoneId: ZoneId, appId: AppId, onlyFolders: OnlyFolders, onlyFiles: OnlyFiles, root: Root, filter: Filter);

            // Get pages from underlying system/provider
            var results = _provider.GetInternal();
            if (results == null || !results.Any())
                return (new List<IEntity>().ToImmutableList(), "null/empty");

            // Convert to Entity-Stream
            _folderBuilder.Configure(appId: AppId, typeName: AppFolderDataRaw.TypeName, titleField: nameof(AppFolderDataRaw.Name));
            var folders = _folderBuilder.CreateMany(_provider.Folders);

            _fileBuilder.Configure(appId: AppId, typeName: AppFileDataRaw.TypeName, titleField: nameof(AppFileDataRaw.Name));
            var files = _fileBuilder.CreateMany(_provider.Files);

            var entities = folders.AddRange(files);

            try
            {
                var asTree = _treeMapper.AddRelationships<string>(entities, "FullName", "Path");

                //var foldersTree = _treeMapper.AddRelationships<string>(folders, "FullName", "Path", "Folder.Folders");
                //var filesTree = _treeMapper.AddRelationships<string>(entities, "FullName", "Path", "Folder.Files");
                //var folderIds = folders.Select(f => f.EntityId).ToList();
                //filesTree = filesTree.Where(f => !folderIds.Contains(f.EntityId)).ToImmutableList();
                // TODO: Handle duplicate entities and empty relations 
                //var asTree = foldersTree.AddRange(filesTree);

                return (asTree, $"As Tree: {asTree.Count}");
            }
            catch (Exception ex)
            {
                l.Ex(ex);

                return (entities, $"Just entities (tree had error): {entities.Count}");
            }
        }));
        private readonly GetOnce<IImmutableList<IEntity>> _getInternal = new GetOnce<IImmutableList<IEntity>>();
    }
}
