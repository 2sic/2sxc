using System;
using ToSic.Eav;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Code
{
    public class BasicDynamicCodeRoot: DynamicCodeRoot
    {
        public BasicDynamicCodeRoot(IServiceProvider serviceProvider, ICmsContext cmsContext) : base(serviceProvider, cmsContext, LogNames.Basic)
        {
        }
    }
}
