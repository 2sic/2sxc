using DotNetNuke.Entities.Modules;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn
{
    public class DnnModuleAndBlockBuilder: ModuleAndBlockBuilder
    {
        public DnnModuleAndBlockBuilder(Generator<IModule> moduleGenerator, GeneratorLog<DnnModuleBlockBuilder> generatorDnnModBlock) : base(DnnConstants.LogName)
        {
            ModuleGenerator = moduleGenerator;
            _generatorDnnModBlock = generatorDnnModBlock.SetLog(Log);
        }
        protected readonly Generator<IModule> ModuleGenerator;
        private readonly GeneratorLog<DnnModuleBlockBuilder> _generatorDnnModBlock;


        protected override IModule GetModuleImplementation(int pageId, int moduleId)
        {
            var moduleInfo = new ModuleController().GetModule(moduleId, pageId, false);
            ThrowIfModuleIsNull(pageId, moduleId, moduleInfo);
            var module = ((DnnModule)ModuleGenerator.New).Init(moduleInfo, Log);
            return module;
        }


        protected override IBlockBuilder GetBuilderImplementation(int pageId, int moduleId)
        {
            var module = GetModuleImplementation(pageId, moduleId);
            return _generatorDnnModBlock.New.GetBlockOfModule((module as DnnModule).GetContents()).BlockBuilder;
        }
    }
}
