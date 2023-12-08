using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Services.CmsService
{
    internal class CmsTweak
    {
        public CmsTweak()
        {
        }

        public CmsTweak ChangeValue(Func<ITweakValue<string>, string> changeFunc)
        {
            return new CmsTweak();
        }
    }
}
