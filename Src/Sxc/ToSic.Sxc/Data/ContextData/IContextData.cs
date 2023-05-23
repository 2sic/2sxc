using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Data
{
    public interface IContextData: IBlockDataSource
    {
        [PrivateApi("WIP")]
        IEntity Default { get; }

        [PrivateApi("WIP")]
        IEntity BlockHeader { get; }

        [PrivateApi("maybe just add for docs")]
        IReadOnlyDictionary<string, IDataStream> Out { get; }
    }
}
