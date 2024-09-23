using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.DataSources;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
#if NETFRAMEWORK
using ToSic.Eav.Apps;
using ToSic.Lib.DI;
using CodeInfoService = ToSic.Eav.Code.InfoSystem.CodeInfoService;
#endif

namespace ToSic.Sxc.DataSources;

/// <summary>
/// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
/// </summary>
/// <remarks>
/// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
/// </remarks>
[PrivateApi("used to be Internal... till 16.01, then changed to private to hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal partial class ContextData : PassThrough, IBlockInstance
{
    #region Constructor and Init

#if NETFRAMEWORK
    public ContextData(MyServices services, IAppReaderFactory appReaders, LazySvc<CodeInfoService> codeChanges) : base(services, "Sxc.BlckDs")
    {
        ConnectLogs([
            _appReaders = appReaders,
            _codeChanges = codeChanges
        ]);
    }
#else

#pragma warning disable IDE0290 // Use primary constructor
    public ContextData(MyServices services) : base(services, "Sxc.BlckDs")
#pragma warning restore IDE0290 // Use primary constructor
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

    public override IReadOnlyDictionary<string, IDataStream> Out => _querySource?.Out ?? base.Out;

}