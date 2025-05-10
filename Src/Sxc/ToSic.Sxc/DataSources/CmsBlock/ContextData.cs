using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.DataSources;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
#if NETFRAMEWORK
using ToSic.Eav.Apps;
using ToSic.Lib.DI;
using CodeInfoService = ToSic.Lib.Code.InfoSystem.CodeInfoService;
#endif

namespace ToSic.Sxc.DataSources;

/// <summary>
/// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
/// </summary>
/// <remarks>
/// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
/// In v19 we removed the implementation of IBlockInstance as it was identical to IDataSource.
/// ...so if anybody had code using that name, it could break, but we assume this is never the case since people would always just use the `Data` object without casting a variable.
/// </remarks>
[PrivateApi("used to be Internal... till 16.01, then changed to private to hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal partial class ContextData : PassThrough
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
    { }
#endif

    #endregion



    #region New v16

    internal IEnumerable<IEntity> MyItems => _myContent.Get(() => _blockSource.GetStream(emptyIfNotFound: true).List);
    private readonly GetOnce<IEnumerable<IEntity>> _myContent = new();

    internal IEnumerable<IEntity> MyHeaders => _header.Get(() => _blockSource.GetStream(ViewParts.StreamHeader, emptyIfNotFound: true).List);
    private readonly GetOnce<IEnumerable<IEntity>> _header = new();
        
    #endregion


    internal void SetOut(Query querySource) => _querySource = querySource;
    private Query _querySource;
    internal void SetBlock(CmsBlock blockSource) => _blockSource = blockSource;
    private CmsBlock _blockSource;

    public override IReadOnlyDictionary<string, IDataStream> Out => _querySource?.Out ?? base.Out;

}