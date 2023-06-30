﻿using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Logging;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public class DynamicCodeRootUnknown: DynamicCodeRoot<object, ServiceKit>
    {
        public DynamicCodeRootUnknown(MyServices services, WarnUseOfUnknown<DynamicCodeRootUnknown> _) : base(services, LogScopes.Base)
        {
        }
    }
}