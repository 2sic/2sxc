using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.Blocks
{
    internal interface ICanHaveBlockContext
    {
        IBlock TryGetBlockContext();
    }
}
