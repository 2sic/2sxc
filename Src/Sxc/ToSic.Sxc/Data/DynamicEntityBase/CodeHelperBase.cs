using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.Data
{
    internal class CodeHelperBase
    {
        public CodeHelperBase(CodeDataFactory cdf, bool strict)
        {
            Cdf = cdf;
            StrictGet = strict;
        }

        public CodeDataFactory Cdf { get; }

        public bool StrictGet { get; }

    }
}
