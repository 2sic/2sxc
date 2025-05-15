using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.Sxc.Services.Kits.Internal;

public interface IExecutionContextCanGetKit
{
    TKit GetKit<TKit>() where TKit : ServiceKit;
}