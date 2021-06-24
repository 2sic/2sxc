using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    internal interface ICanGetNameNotFinal
    {
        dynamic Get(string name);
    }
}
