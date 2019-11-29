using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// This marks data sources which are meant for Blocks (Modules, Content-Block Instances). <br/>
    /// They have some internal functionality which isn't published as of now.
    /// </summary>
    [PublicApi]
    public interface IBlockDataSource: IDataSource, IDataTarget
    {
        [PrivateApi("older use case, will probably become obsolete some day")]
        DataPublishing Publish { get; }

    }
}