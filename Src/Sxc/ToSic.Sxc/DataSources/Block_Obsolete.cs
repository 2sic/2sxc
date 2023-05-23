using ToSic.Lib.Documentation;

namespace ToSic.Sxc.DataSources
{
    public partial class Block
    {
#if NETFRAMEWORK
#pragma warning disable 618
        [System.Obsolete("Old property on this data source, should really not be used at all. Must add warning in v13, and remove ca. v15")]
        [PrivateApi]
        public Compatibility.CacheWithGetContentType Cache
        {
            get
            {
                if (_cache != null) return _cache;
                Compatibility.Obsolete.Warning13To15("Data.Cache", "", "https://r.2sxc.org/brc-13-datasource-cache");
                return _cache = new Compatibility.CacheWithGetContentType(_appStates.Get(this));
            }
        }

        [System.Obsolete]
        private Compatibility.CacheWithGetContentType _cache;
#pragma warning restore 618
#endif
    }
}
