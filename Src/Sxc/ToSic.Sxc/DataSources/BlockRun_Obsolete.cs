#if NETFRAMEWORK
using ToSic.Eav.Apps;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Lib.DI;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.DataSources.Internal.Compatibility;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;

namespace ToSic.Sxc.DataSources;

partial class ContextData: IBlockDataSource
{
    private readonly LazySvc<CodeInfoService> _codeChanges;

    private readonly IAppReaderFactory _appReaders;

#pragma warning disable 618
    [System.Obsolete("Old property on this data source, should really not be used at all. Must add warning in v13, and remove ca. v15")]
    [PrivateApi]
    public CacheWithGetContentType Cache
    {
        get
        {
            if (_cache != null) return _cache;
            // on first access report problem
            _codeChanges.Value.Warn(CaV8To17("Data.Cache", "https://go.2sxc.org/brc-13-datasource-cache"));
            return _cache = new(_appReaders.Get(this));
        }
    }

    [System.Obsolete]
    private CacheWithGetContentType _cache;

    [PrivateApi("older use case, probably don't publish")]
    public DataPublishing Publish { get; } = new();
#pragma warning restore 618
    
}

#endif
