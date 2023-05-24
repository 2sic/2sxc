using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// This marks data sources which are meant for Blocks (Modules, Content-Block Instances). <br/>
    /// They have some internal functionality which isn't published as of now.
    /// </summary>
    [PrivateApi("used to be PublicApi_Stable_ForUseInYourCode till 16.01, but replaced by IContextData")]
    public interface IBlockDataSource: IDataSource
    {

        [PrivateApi("older use case, will probably become obsolete some day")]
        DataPublishing Publish { get; }

#if NETFRAMEWORK
        [System.Obsolete("Must be removed soon, but it's part of older Mobius so we must add warnings there")]
        [PrivateApi]
        Compatibility.CacheWithGetContentType Cache { get; }
#endif
    }
}