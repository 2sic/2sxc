#if NETFRAMEWORK
using ToSic.Eav.Apps;
using ToSic.Sxc.DataSources.Internal.Compatibility;
using ToSic.Sys.Code.InfoSystem;
using static ToSic.Sys.Code.Infos.CodeInfoObsolete;

namespace ToSic.Sxc.DataSources;

partial class ContextData: IBlockDataSource
{
    private readonly LazySvc<CodeInfoService> _codeChanges;

    private readonly IAppReaderFactory _appReaders;

#pragma warning disable 618
    [Obsolete("Old property on this data source, should really not be used at all. Must add warning in v13, and remove ca. v15")]
    [PrivateApi]
    public CacheWithGetContentType Cache
    {
        get
        {
            if (field != null)
                return field;
            // on first access report problem
            _codeChanges.Value.Warn(CaV8To17("Data.Cache", "https://go.2sxc.org/brc-13-datasource-cache"));
            return field = new(_appReaders.Get(this));
        }
    }

    [PrivateApi("older use case, probably don't publish")]
    public DataPublishing Publish { get; } = new();
#pragma warning restore 618
    
}

#endif
