using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
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
        NiceName = "Adam",
        UiHint = "Files and folders in the Adam",
        GlobalName = "ee1d0cb6-5086-4d59-b16a-d0dc7b594bf2",
        HelpLink = "https://r.2sxc.org/ds-adam",
        Icon = Icons.Tree,
        Type = DataSourceType.Lookup,
        Audience = Audience.Advanced,
        In = new[] { QueryConstants.InStreamDefaultRequired },
        DynamicOut = false,
        ConfigurationType = "" // TODO: ...
        )]
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP")]
    public class Adam : DataSource
    {
        private readonly IDataFactory _dataFactory;
        private readonly AdamDataSourceProvider<int, int> _provider;

        #region Configuration properties

        [Configuration]
        public string EntityIds
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        [Configuration]
        public string EntityGuids
        {
            get => Configuration.GetThis();
            set => Configuration.SetThis(value);
        }

        [Configuration]
        public string Fields
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
        public Adam(MyServices services, AdamDataSourceProvider<int, int> provider, IDataFactory dataDataFactory) : base(services, "CDS.Adam")
        {
            ConnectServices(
                _provider = provider,
                _dataFactory = dataDataFactory
            );

            ProvideOut(GetDefault);
            ProvideOut(GetFolders, "Folders");
            ProvideOut(GetFiles, "Files");
        }
        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetDefault() => GetInternal();

        [PrivateApi]
        public IImmutableList<IEntity> GetFolders() => GetInternal().Where(e => e.GetBestValue<bool>("IsFolder", null)).ToImmutableList();

        [PrivateApi]
        public IImmutableList<IEntity> GetFiles() => GetInternal().Where(e => !e.GetBestValue<bool>("IsFolder", null)).ToImmutableList();

        private IImmutableList<IEntity> GetInternal() => _getInternal.Get(() => Log.Func(l =>
        {
            Configuration.Parse();

            // Make sure we have an In - otherwise error
            var source = TryGetIn();
            if (source is null) return (Error.TryGetInFailed(this), "error");

            _provider.Configure(appId: AppId, entityIds: EntityIds, entityGuids: EntityGuids, fields: Fields, filter: Filter);
            var find = _provider.GetInternal();

            var adamFactory = _dataFactory.New(options: new DataFactoryOptions(AdamItemDataRaw.Options, appId: AppId));

            var entities = adamFactory.Create(source.SelectMany(o => find(o)));

            return (entities, "ok");
        }));
        private readonly GetOnce<IImmutableList<IEntity>> _getInternal = new GetOnce<IImmutableList<IEntity>>();

    }
}
