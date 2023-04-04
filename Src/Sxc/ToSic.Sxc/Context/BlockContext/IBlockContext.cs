using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// WIP experimental v15.07+ - goal is to provide block data etc. to a view...
    ///
    /// 2dm is working on this, nothing real to use yet...
    /// </summary>
    public interface IBlockContext
    {
        IEntity Item { get; }
        
        /// <summary>
        /// Probably the default data / aka "Content"
        /// </summary>
        IEnumerable<IEntity> Items { get; }

        IEntity Header { get; }

        IEnumerable<IEntity> Headers { get; }
    }
}
