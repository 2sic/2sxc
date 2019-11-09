using ToSic.Eav.DataSources;
using ToSic.SexyContent;

namespace ToSic.Sxc.Blocks
{
    public interface IBlockDataSource: IDataSource, IDataTarget
    {

        DataPublishing Publish { get; }

    }
}