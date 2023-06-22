using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Eav.CodeChanges;
using ToSic.Lib.DI;
using static ToSic.Sxc.Code.DynamicCode16Warnings;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
    /// </summary>
    [PrivateApi("used to be Internal... till 16.01, then changed to private to hide implementation")]
    internal partial class ContextData : PassThrough, IContextData
    {
        #region Constructor and Init

#if NETFRAMEWORK
        [PrivateApi("not meant for public use")]
        public ContextData(MyServices services, ToSic.Eav.Apps.IAppStates appStates, LazySvc<CodeChangeService> codeChanges) : base(services, "Sxc.BlckDs")
        {
            ConnectServices(
                _appStates = appStates,
                _codeChanges = codeChanges
            );
        }

        private readonly ToSic.Eav.Apps.IAppStates _appStates;
#else
        [PrivateApi("not meant for public use")]
        public ContextData(MyServices services, LazySvc<CodeChangeService> codeChanges) : base(services, "Sxc.BlckDs")
        {
            ConnectServices(
                _codeChanges = codeChanges
            );
        }
#endif
        private readonly LazySvc<CodeChangeService> _codeChanges;

        #endregion

        #region New v16

        public IEnumerable<IEntity> MyContent =>  _myContent.Get(() => _blockSource.GetStream(emptyIfNotFound: true).List);

        IEnumerable<IEntity> IContextData.MyContent => _codeChanges.Value.GetAndWarn(NoDataMyContent, MyContent);
        private readonly GetOnce<IEnumerable<IEntity>> _myContent = new GetOnce<IEnumerable<IEntity>>();

        public IEnumerable<IEntity> MyData => _myData.Get(() => GetStream(emptyIfNotFound: true).List);

        IEnumerable<IEntity> IContextData.MyData => _codeChanges.Value.GetAndWarn(NoDataMyData, MyData);
        private readonly GetOnce<IEnumerable<IEntity>> _myData = new GetOnce<IEnumerable<IEntity>>();

        public IEnumerable<IEntity> MyHeader =>  _header.Get(() => _blockSource.GetStream(ViewParts.StreamHeader, emptyIfNotFound: true).List);
        IEnumerable<IEntity> IContextData.MyHeader => _codeChanges.Value.GetAndWarn(NoDataMyHeader, MyHeader);
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