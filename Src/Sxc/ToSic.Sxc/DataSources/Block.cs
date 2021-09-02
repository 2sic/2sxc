using System;
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

#pragma warning disable 618
        [Obsolete("Old property on this data source, should really not be used at all. Will be removed in 2sxc 12")]
        [PrivateApi]
        public CacheWithGetContentType Cache => _cache ?? (_cache = new CacheWithGetContentType(Eav.Apps.State.Get(this)));
        [Obsolete]
        private CacheWithGetContentType _cache;
#pragma warning restore 618

        
        internal void SetOut(Query querySource) => Out = querySource.Out;
    }
}