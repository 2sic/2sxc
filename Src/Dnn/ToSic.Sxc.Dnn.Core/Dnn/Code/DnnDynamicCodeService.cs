using System;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Code
{
    [PrivateApi("hide implementation")]
    public class DnnDynamicCodeService: DynamicCodeService
    {
        public DnnDynamicCodeService(
            IServiceProvider serviceProvider, 
            Lazy<LogHistory> history)
            : base(serviceProvider, history, DnnConstants.LogName)
        {
            // Must use the ServiceProvider of the base class to generate these, 
            // Otherwise it's in the wrong scope!
            _blockGenerator = ServiceProvider.Build<Generator<DnnModuleBlockBuilder>>();
        }
        private readonly Generator<DnnModuleBlockBuilder> _blockGenerator;


        public override IDynamicCode12 OfModule(int pageId, int moduleId)
        {
            var wrapLog = Log.Call<IDynamicCodeRoot>($"{pageId}, {moduleId}");
            MakeSureLogIsInHistory();
            var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false);
            if (moduleInfo == null)
            {
                var msg = $"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?";
                Log.Add(msg);
                throw new Exception(msg);
            }
            var cmsBlock = _blockGenerator.New.Init(Log).GetBlockOfModule(moduleInfo);
            var codeRoot = CodeRootGenerator.New.Init(cmsBlock, Log, Constants.CompatibilityLevel12);

            return wrapLog("ok", codeRoot);
        }

    }
}
