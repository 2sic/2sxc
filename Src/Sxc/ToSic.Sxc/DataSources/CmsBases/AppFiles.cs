using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// Important Info to people working with this
// It depends on abstract provder, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of App files from the current platform (Dnn or Oqtane).
    ///
    /// As of now there are no parameters to set.
    ///
    /// To figure out the properties returned and what they match up to, see <see cref="CmsPageInfo"/>
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = "", // TODO:
        GlobalName = "3fe6c215-4c37-45c1-8883-b4b2a47162a7",
        HelpLink = "https://r.2sxc.org/ds-appfiles",
        Icon = Icons.Tree,
        NiceName = "AppFiles",
        Type = DataSourceType.Source,
        Difficulty = DifficultyBeta.Advanced,
        UiHint = "Files and folders in the App folder")]
    public class AppFiles: ExternalData
    {
        private readonly ITreeMapper _treeMapper;
        private readonly IDataBuilder _fileBuilder;
        private readonly AppFilesDataSourceProvider _provider;

        #region Configuration properties

        [Configuration]
        public bool OnlyFolders
        {
            get => Configuration.GetThis(false);
            set => Configuration.SetThis(value);
        }

        [Configuration]
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
        public AppFiles(Dependencies services, AppFilesDataSourceProvider provider, IDataBuilder dataBuilder, ITreeMapper treeMapper) : base(services, "CDS.AppFiles")
        {
            ConnectServices(
                _provider = provider,
                _fileBuilder = dataBuilder.Configure(typeName: AppFileInfo.TypeName, titleField: nameof(AppFileInfo.Title)),
                _treeMapper = treeMapper
            );

            Provide(GetDefault);
            Provide("Folders", GetFolders);
            Provide("Files", GetFiles);
        }
        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetDefault() => GetInternal(onlyFiles: OnlyFiles, onlyFolders: OnlyFolders);

        [PrivateApi]
        public IImmutableList<IEntity> GetFolders() => GetInternal(onlyFiles: false, onlyFolders: true);

        [PrivateApi]
        public IImmutableList<IEntity> GetFiles() => GetInternal(onlyFiles: true, onlyFolders: false);

        private IImmutableList<IEntity> GetInternal(bool onlyFolders, bool onlyFiles) => Log.Func(l =>
        {
            Configuration.Parse();

            // Get pages from underlying system/provider
            var appFiles = _provider.GetAppFilesInternal(
                zoneId: ZoneId,
                appId: AppId,
                onlyFolders: onlyFolders,
                onlyFiles: onlyFiles,
                root: Root,
                filter: Filter
            );
            if (appFiles == null || !appFiles.Any())
                return (new List<IEntity>().ToImmutableList(), "null/empty");

            // Convert to Entity-Stream
            var entities = _fileBuilder.CreateMany(appFiles);

            try
            {
                var asTree = _treeMapper.AddRelationships<int>(entities, "EntityId", "ParentId", "Children", "Parent");
                return (asTree, $"As Tree: {asTree.Count}");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (entities, $"Just entities (tree had error): {entities.Count}");
            }
        });
    }
}
