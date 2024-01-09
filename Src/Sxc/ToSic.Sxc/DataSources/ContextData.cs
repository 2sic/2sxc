using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data;
#if NETFRAMEWORK
using ToSic.Lib.DI;
using CodeInfoService = ToSic.Eav.Code.InfoSystem.CodeInfoService;
#endif

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
    /// </summary>
    [PrivateApi("used to be Internal... till 16.01, then changed to private to hide implementation")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal partial class ContextData : PassThrough, IContextData
    {
        #region Constructor and Init

#if NETFRAMEWORK
        [PrivateApi("not meant for public use")]
        public ContextData(MyServices services, ToSic.Eav.Apps.IAppStates appStates, LazySvc<CodeInfoService> codeChanges) : base(services, "Sxc.BlckDs")
        {
            ConnectServices(
                _appStates = appStates,
                _codeChanges = codeChanges
            );
        }
#else
        [PrivateApi("not meant for public use")]
        public ContextData(MyServices services) : base(services, "Sxc.BlckDs")
        {
        }
#endif

        #endregion



        #region New v16

        internal IEnumerable<IEntity> MyItem => _myContent.Get(() => _blockSource.GetStream(emptyIfNotFound: true).List);
        private readonly GetOnce<IEnumerable<IEntity>> _myContent = new();

        internal IEnumerable<IEntity> MyHeader => _header.Get(() => _blockSource.GetStream(ViewParts.StreamHeader, emptyIfNotFound: true).List);
        private readonly GetOnce<IEnumerable<IEntity>> _header = new();
        
        #endregion


        internal void SetOut(Query querySource) => _querySource = querySource;
        private Query _querySource;
        internal void SetBlock(CmsBlock blockSource) => _blockSource = blockSource;
        private CmsBlock _blockSource;

        // #DataInAddWontWork
        // private IReadOnlyDictionary<string, IDataStream> PickOut => _querySource?.Out ?? base.Out;

        public override IReadOnlyDictionary<string, IDataStream> Out => _querySource?.Out ?? base.Out;

    }
}