using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.Code.Errors
{
    public interface IExceptionWithHelp
    {
        CodeError Help { get; }
    }
}
