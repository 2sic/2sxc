#if NETFRAMEWORK
using ToSic.Lib.Documentation;
using static ToSic.Eav.CodeChanges.CodeChangeInfo;

namespace ToSic.Sxc.DataSources
{
    internal partial class ContextData
    {
#pragma warning disable 618
        [System.Obsolete("Old property on this data source, should really not be used at all. Must add warning in v13, and remove ca. v15")]
        [PrivateApi]
        public Compatibility.CacheWithGetContentType Cache
        {
            get
            {
                if (_cache != null) return _cache;
                // on first access report problem
                _codeChanges.Value.Warn(V13To17("Data.Cache", "https://go.2sxc.org/brc-13-datasource-cache"));
                return _cache = new Compatibility.CacheWithGetContentType(_appStates.Get(this));
            }
        }

        [System.Obsolete]
        private Compatibility.CacheWithGetContentType _cache;
#pragma warning restore 618
    }
}
#endif
