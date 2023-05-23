using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public partial class Block : PassThrough, IBlockDataSource
    {
        public IEntity Default { get; }
        private readonly GetOnce<IEntity> _default = new GetOnce<IEntity>();
        public IEntity Header { get; }
        private readonly GetOnce<IEntity> _header = new GetOnce<IEntity>();

        [PrivateApi("older use case, probably don't publish")]
        public DataPublishing Publish { get; }= new DataPublishing();

        internal void SetOut(Query querySource) => _querySource = querySource;
        private Query _querySource;

        public override IReadOnlyDictionary<string, IDataStream> Out => _querySource?.Out ?? base.Out;

        [PrivateApi("not meant for public use")]
        public Block(MyServices services, IAppStates appStates) : base(services, "Sxc.BlckDs") => _appStates = appStates;
        private readonly IAppStates _appStates;


    }
}