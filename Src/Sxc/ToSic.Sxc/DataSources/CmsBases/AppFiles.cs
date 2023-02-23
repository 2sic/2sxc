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
using static System.StringComparer;

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
        // Problems
        // 1. Guids look real, so devs my try to use them - but they will change on each request
        // 2. merging and splitting data by filtering type again is not efficient
        //    - Solution: only execute once, keep the three streams in one variable and just take the parts you need
        private readonly IDataBuilder _folderBuilder;
        private readonly ITreeMapper _treeMapper;
        private readonly IDataBuilder _fileBuilder;
        private readonly AppFilesDataSourceProvider _provider;

        private const string StreamFiles = "Files";
        private const string StreamFolders = "Folders";

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

            Provide(() => GetMultiAccess(Eav.Constants.DefaultStreamName));
            Provide(StreamFolders, () => GetMultiAccess(StreamFolders));
            Provide(StreamFiles, () => GetMultiAccess(StreamFiles));
        }
        #endregion

        /// <summary>
        /// Mini-cache.
        /// Reason is that we can only generate the streams together, so this ensures that after generating them once,
        /// other streams requested at the same time won't re-trigger stream generation.
        /// </summary>
        private IImmutableList<IEntity> GetMultiAccess(string streamName) => _multiAccess.Get(() =>
        {
            var (folders, files) = GetInternal();
            return new Dictionary<string, IImmutableList<IEntity>>(OrdinalIgnoreCase)
            {
                { Eav.Constants.DefaultStreamName, folders.Concat(files).ToImmutableList() },
                { StreamFolders, folders.ToImmutableList() },
                { StreamFiles, files.ToImmutableList() }
            };
        })[streamName];
        private readonly GetOnce<Dictionary<string, IImmutableList<IEntity>>> _multiAccess = new GetOnce<Dictionary<string, IImmutableList<IEntity>>>();

        private (ICollection<IEntity> folders, ICollection<IEntity> files) GetInternal() => Log.Func(l =>
        {
            Configuration.Parse();

            _provider.Configure(zoneId: ZoneId, appId: AppId, onlyFolders: OnlyFolders, onlyFiles: OnlyFiles, root: Root, filter: Filter);

            // Get pages from underlying system/provider
            var results = _provider.GetInternal();
            if (results == null || !results.Any())
                return ((EmptyList.ToList(), EmptyList.ToList()), "null/empty");

            // Convert to Entity-Stream
            _folderBuilder.Configure(appId: AppId, typeName: AppFolderDataRaw.TypeName, titleField: nameof(AppFolderDataRaw.Name));
            var folders = _folderBuilder.Prepare(_provider.Folders);
            l.A($"Folders: {folders.Count}");

            _fileBuilder.Configure(appId: AppId, typeName: AppFileDataRaw.TypeName, titleField: nameof(AppFileDataRaw.Name));
            var files = _fileBuilder.Prepare(_provider.Files);
            l.A($"Files: {files.Count}");


            try
            {
                // First prepare subfolder list for each folder
                var folderNeeds = folders
                    .Select(pair => (pair.Key, pair.Value, new List<string> { pair.Key.FullName }))
                    .ToList();

                var foldersLookup = folders
                    .Select(pair => (pair.Value, pair.Key.ParentFolderInternal))
                    .ToList();

                var foldersWithFirstTree = _treeMapper
                    .AddOneRelationship("Folders", folderNeeds, foldersLookup, cloneFirst: false);


                // Second prepare files list for each folder

                //var folderNeedsFiles = foldersWithFirstTree
                //    .Select(pair =>
                //    {
                //        var filesInFolder = _provider.Files
                //            .Where(f => f.Path.Equals(pair.Key.FullName))
                //            .Select(f => f.Guid)
                //            .ToList();
                //        return (pair.Key, pair.Value, filesInFolder);
                //    }).ToList();

                //var filesLookup = files.Values.Select(entity => (entity, entity.EntityGuid)).ToList();
                var folderNeedsFiles = foldersWithFirstTree
                    .Select(pair => (pair.Key, pair.Value, new List<string> { pair.Key.FullName }))
                    .ToList();

                var filesLookup = files.Select(pair => (pair.Value, pair.Key.ParentFolderInternal)).ToList();


                var foldersWithSecondTreeAlso =
                    _treeMapper.AddOneRelationship("Files", folderNeedsFiles, filesLookup, cloneFirst: false);

                // add files to final results
                return ((foldersWithSecondTreeAlso.Values, files.Values), "ok");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return ((folders.Values, files.Values), "error");
            }
        });
    }
}
