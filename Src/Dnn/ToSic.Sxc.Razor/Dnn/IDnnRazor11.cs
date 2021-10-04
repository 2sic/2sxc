using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn
{
    [PrivateApi]
    public interface IDnnRazor11
    {
        /// <summary>
        /// Code-Behind of this .cshtml file - located in a file with the same name but ending in .code.cshtml
        /// </summary>
        dynamic Code { get; }
    }
}
