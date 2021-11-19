using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Services
{
    [PrivateApi("WIP 13")]
    public interface IToolbarRuleService
    {
        string Metadata(
            object target,
            string contentType);
    }
}
