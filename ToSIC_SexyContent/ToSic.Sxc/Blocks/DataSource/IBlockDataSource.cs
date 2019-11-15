using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// A special data-source for a block, which also knows about data-publishing (to ensure page-versioning if necessary). <br/>
    /// It's not documented more, as we may still make changes to it.
    /// </summary>
    [PrivateApi("not sure yet if it should be in blocks, or DataSources")]
    public interface IBlockDataSource: IDataSource, IDataTarget
    {
        [PrivateApi]
        DataPublishing Publish { get; }

    }
}