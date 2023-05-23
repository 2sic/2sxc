using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;
using static ToSic.Eav.DataSource.DataSourceConstants;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
    /// </summary>
    [PrivateApi("used to be Internal... till 16.01")]
    public partial class Block : PassThrough, IContextData
    {
        #region New v16

        [PrivateApi]
        public IEntity Content => _default.Get(() => TryToGetFirstOfStream(_blockSource, StreamDefaultName));
        private readonly GetOnce<IEntity> _default = new GetOnce<IEntity>();

        [PrivateApi]
        public IEntity Header => _header.Get(() => TryToGetFirstOfStream(_blockSource, ViewParts.StreamHeader));
        private readonly GetOnce<IEntity> _header = new GetOnce<IEntity>();

        [PrivateApi]
        internal IEntity TryToGetFirstOfStream(IDataSource source, string streamName)
        {
            var wrapLog = Log.Fn<IEntity>(streamName);
            var list = source.GetStream(streamName, nullIfNotFound: true)?.List?.ToList();
            if (list == null) return wrapLog.ReturnNull("stream not found");

            return list.Any()
                ? wrapLog.Return(list.FirstOrDefault(), "found")
                : wrapLog.ReturnNull("first is null");
        }

        #endregion


        [PrivateApi("older use case, probably don't publish")]
        public DataPublishing Publish { get; }= new DataPublishing();

        internal void SetOut(Query querySource) => _querySource = querySource;
        private Query _querySource;
        internal void SetBlock(CmsBlock blockSource) => _blockSource = blockSource;
        private CmsBlock _blockSource;

        public override IReadOnlyDictionary<string, IDataStream> Out => _querySource?.Out ?? base.Out;

        [PrivateApi("not meant for public use")]
        public Block(MyServices services, IAppStates appStates) : base(services, "Sxc.BlckDs") => _appStates = appStates;
        private readonly IAppStates _appStates;


    }
}