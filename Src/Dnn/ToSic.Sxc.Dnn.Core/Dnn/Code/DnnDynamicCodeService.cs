using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.Code
{
    [PrivateApi("hide implementation")]
    public class DnnDynamicCodeService: DynamicCodeService
    {
        public DnnDynamicCodeService(Dependencies dependencies) : base(dependencies, DnnConstants.LogName)
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
            var module = ModuleAndBlockBuilder.Ready.GetModule(pageId, moduleId);
            var cmsBlock = _blockGenerator.New.Init(Log).GetBlockOfModule((module as DnnModule)?.GetContents());
            var codeRoot = CodeRootGenerator.New.Init(cmsBlock, Log, Constants.CompatibilityLevel12);

            return wrapLog("ok", codeRoot);
        }

    }
}
