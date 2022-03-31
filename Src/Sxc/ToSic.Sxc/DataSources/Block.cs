using System;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// The main data source for Blocks. Internally often uses <see cref="CmsBlock"/> to find what it should provide.
    /// It's based on the <see cref="PassThrough"/> data source, because it's just a coordination-wrapper.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public class Block : PassThrough, IBlockDataSource
    {

        [PrivateApi]
        public override string LogId => "Sxc.BlckDs";

        [PrivateApi("older use case, probably don't publish")]
        public DataPublishing Publish { get; }= new DataPublishing();

        internal void SetOut(Query querySource) => Out = querySource.Out;

        [PrivateApi("not meant for public use")]
        public Block(IAppStates appStates) => _appStates = appStates;
        private readonly IAppStates _appStates;

#if NETFRAMEWORK
#pragma warning disable 618
        [Obsolete("Old property on this data source, should really not be used at all. Must add warning in v13, and remove ca. v15")]
        [PrivateApi]
        public CacheWithGetContentType Cache
        {
            get
            {
                if (_cache != null) return _cache;
                Obsolete.Warning13To15(nameof(Cache), "", "https://r.2sxc.org/brc-13-datasource-cache");
                return _cache = new CacheWithGetContentType(_appStates.Get(this));
            }
        }

        [Obsolete]
        private CacheWithGetContentType _cache;
#pragma warning restore 618
#endif
    }
}