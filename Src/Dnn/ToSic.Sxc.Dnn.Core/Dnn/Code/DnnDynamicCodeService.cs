using System;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Code
{
    public class DnnDynamicCodeService: DynamicCodeService
    {
        public DnnDynamicCodeService(IServiceProvider serviceProvider, Lazy<LogHistory> history) 
            : base(serviceProvider, history, DnnConstants.LogName)
        {
        }

        public override IDynamicCode12 OfModule(int pageId, int moduleId)
        {
            var wrapLog = Log.Call<IDynamicCodeRoot>($"{pageId}, {moduleId}");
            var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false);
            if (moduleInfo == null)
            {
                var msg = $"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?";
                Log.Add(msg);
                throw new Exception(msg);
            }
            var cmsBlock = ServiceProvider.Build<DnnModuleBlockBuilder>().Init(Log).GetBlockOfModule(moduleInfo);

            var codeRoot = ServiceProvider.Build<DnnDynamicCodeRoot>().Init(cmsBlock, Log, Constants.CompatibilityLevel10) as DnnDynamicCodeRoot;

            return wrapLog("ok", codeRoot);
        }
    }
}
