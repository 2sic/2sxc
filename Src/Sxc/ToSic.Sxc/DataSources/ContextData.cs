using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
    /// </summary>
    [PrivateApi("used to be Internal... till 16.01, then changed to private to hide implementation")]
    internal partial class ContextData : PassThrough, IContextData
    {
        [PrivateApi("not meant for public use")]
        public ContextData(MyServices services, IAppStates appStates) : base(services, "Sxc.BlckDs") => _appStates = appStates;
        private readonly IAppStates _appStates;

        #region New v16

        public IEnumerable<IEntity> MyContent => _myContent.Get(() => _blockSource.GetStream(emptyIfNotFound: true).List);
        private readonly GetOnce<IEnumerable<IEntity>> _myContent = new GetOnce<IEnumerable<IEntity>>();

        public IEnumerable<IEntity> MyData => _myData.Get(() => GetStream(emptyIfNotFound: true).List);
        private readonly GetOnce<IEnumerable<IEntity>> _myData = new GetOnce<IEnumerable<IEntity>>();

        public IEnumerable<IEntity> MyHeader => _header.Get(() => _blockSource.GetStream(ViewParts.StreamHeader, emptyIfNotFound: true).List);
        private readonly GetOnce<IEnumerable<IEntity>> _header = new GetOnce<IEnumerable<IEntity>>();

        #endregion


        [PrivateApi("older use case, probably don't publish")]
        public DataPublishing Publish { get; } = new DataPublishing();

        internal void SetOut(Query querySource) => _querySource = querySource;
        private Query _querySource;
        internal void SetBlock(CmsBlock blockSource) => _blockSource = blockSource;
        private CmsBlock _blockSource;

        public override IReadOnlyDictionary<string, IDataStream> Out => _querySource?.Out ?? base.Out;


    }
}