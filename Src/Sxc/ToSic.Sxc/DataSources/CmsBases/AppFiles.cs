using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Process;
using ToSic.Eav.Data.Source;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using static System.StringComparer;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
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
        private readonly IDataFactory _folderFactory;
        private readonly ITreeMapper _treeMapper;
        private readonly IDataFactory _fileFactory;
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
        public AppFiles(MyServices services, AppFilesDataSourceProvider provider, IDataFactory filesFactory, IDataFactory foldersFactory, ITreeMapper treeMapper) : base(services, "CDS.AppFiles")
        {
            ConnectServices(
                _provider = provider,
                _fileFactory = filesFactory,
                _folderFactory = foldersFactory,
                _treeMapper = treeMapper
            );

            Provide(() => GetMultiAccess(Eav.Constants.DefaultStreamName));
            Provide(() => GetMultiAccess(StreamFolders), StreamFolders);
            Provide(() => GetMultiAccess(StreamFiles), StreamFiles);
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
                { StreamFolders, folders },
                { StreamFiles, files }
            };
        })[streamName];
        private readonly GetOnce<Dictionary<string, IImmutableList<IEntity>>> _multiAccess = new GetOnce<Dictionary<string, IImmutableList<IEntity>>>();

        /// <summary>
        /// Get both the files and folders stream
        /// </summary>
        /// <returns></returns>
        private (IImmutableList<IEntity> folders, IImmutableList<IEntity> files) GetInternal() => Log.Func(l =>
        {
            Configuration.Parse();

            _provider.Configure(zoneId: ZoneId, appId: AppId, onlyFolders: OnlyFolders, onlyFiles: OnlyFiles, root: Root, filter: Filter);

            // Get pages from underlying system/provider
            var (rawFolders, rawFiles) = _provider.GetInternal();
            if (!rawFiles.Any() && !rawFolders.Any())
                return ((EmptyList, EmptyList), "null/empty");

            // Convert to Entity-Stream
            _folderFactory.Configure(appId: AppId, typeName: AppFolderDataRaw.TypeName, titleField: nameof(AppFolderDataRaw.Name));
            // WIP CONTINUE HERE!!! @2DM - JUST CHANGED, SEEMS TO WORK!!!
            var masterLookup = new LazyLookup<string, IEntity>();
            ((DataFactory)_folderFactory).LookupWip = masterLookup;
            var folders = _folderFactory.Prepare(rawFolders);
            l.A($"Folders: {folders.Count}");

            _fileFactory.Configure(appId: AppId, typeName: AppFileDataRaw.TypeName, titleField: nameof(AppFileDataRaw.Name));
            var files = _fileFactory.Prepare(rawFiles);
            l.A($"Files: {files.Count}");


            try
            {
                // New WIP
                //var pathFolderLookup = new LazyLookup<string, IEntity>();
                //var parentPathFolderLookup = new LazyLookup<string, IEntity>();
                //var pathFileLookup = new LazyLookup<string, IEntity>();

                var tm = (TreeMapper)_treeMapper;
                // Get Triplet of folder with search params for the Parent Folder, then attach relationship and keep modified result
                var foldersUpd = folders
                    .Select(pair => pair.Extend(pair.Partner.NeedsParentWIP)).ToList();
                foldersUpd = tm.AddOneRelationshipWIP("Parent", foldersUpd, masterLookup);

                // Now create Triplet of Folder with Search Params for everything that has it's full name, keep modified result
                var folderNeedsInbound2 = foldersUpd
                    .Select(pair => pair.Extend(pair.Partner.NeedsChildFoldersWip)).ToList();
                foldersUpd = tm.AddOneRelationshipWIP("Folders", folderNeedsInbound2, masterLookup);
                var folderNeedsFiles = foldersUpd
                    .Select(pair => pair.Extend(pair.Partner.NeedsChildFilesWip)).ToList();
                foldersUpd = tm.AddOneRelationshipWIP("Files", folderNeedsFiles, masterLookup);

                // var foldersLooped = foldersUpd.

                var filesUpd = tm.AddOneRelationshipWIP("Parent", files, masterLookup, pair => pair.Partner.NeedsParentWIP);


                var lookupEverything2 =
                    ((DataFactory)_folderFactory).GenerateLookup(
                        filesUpd.Select(f => new EntityPair<IRawEntity>(f.Entity, f.Partner)),
                        foldersUpd.Select(f => new EntityPair<IRawEntity>(f.Entity, f.Partner))
                    );

                masterLookup.Update(lookupEverything2);
                    

                // Return the final streams
                return ((_folderFactory.WrapUp(foldersUpd), _fileFactory.WrapUp(filesUpd)), "ok");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return ((_folderFactory.WrapUp(folders), _fileFactory.WrapUp(files)), "error");
            }
        });


    }
}
