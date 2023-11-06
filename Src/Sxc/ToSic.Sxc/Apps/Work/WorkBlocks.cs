using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.Data.Build;

namespace ToSic.Eav.Apps.Work
{
    public class WorkBlocks : WorkUnitBase<IAppWorkCtxWithDb>
    {
        private readonly AppWork _appWork;
        private readonly DataBuilder _builder;

        public WorkBlocks(AppWork appWork, DataBuilder builder) : base("AWk.EntCre")
        {
            ConnectServices(
                _appWork = appWork,
                _builder = builder
            );
        }
        
    }
}
