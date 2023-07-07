#if NETFRAMEWORK
using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    public interface IBlockDataSourceOld: IDataSource
    {
        [PrivateApi("older use case, will probably become obsolete some day")]
        DataPublishing Publish { get; }

        [System.Obsolete("Must be removed soon, but it's part of older Mobius so we must add warnings there")]
        [PrivateApi]
        Compatibility.CacheWithGetContentType Cache { get; }

        // #DataInAddWontWork
        //new IDictionary<string, IDataStream> In { get; }

    }
}
#endif
