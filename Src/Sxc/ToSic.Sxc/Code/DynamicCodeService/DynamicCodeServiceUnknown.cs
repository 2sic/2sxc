using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Code
{
    public class DynamicCodeServiceUnknown: DynamicCodeService
    {
        public DynamicCodeServiceUnknown(IServiceProvider serviceProvider, Lazy<LogHistory> history, WarnUseOfUnknown<DynamicCodeServiceUnknown> warn) 
            : base(serviceProvider, history, "Unk")
        {
        }

        public override IDynamicCode12 OfModule(int pageId, int moduleId) => throw new NotSupportedException();
    }
}
