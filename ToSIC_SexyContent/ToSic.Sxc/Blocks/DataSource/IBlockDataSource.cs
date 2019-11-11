using ToSic.Eav.DataSources;
using ToSic.Eav.Documentation;
using ToSic.SexyContent;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// A special data-source for a block, which also knows about data-publishing (to ensure page-versioning if necessary). <br/>
    /// It's not documented more, as we may still make changes to it.
    /// </summary>
    [PublicApi]
    public interface IBlockDataSource: IDataSource, IDataTarget
    {
        [PrivateApi]
        DataPublishing Publish { get; }

    }
}