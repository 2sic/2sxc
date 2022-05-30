using System;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    public class ModuleAndBlockBuilderUnknown: ModuleAndBlockBuilder
    {
        public ModuleAndBlockBuilderUnknown(WarnUseOfUnknown<ModuleAndBlockBuilderUnknown> warn) : base("Unk")
        {
        }

        protected override IModule GetModuleImplementation(int pageId, int moduleId) => throw new NotSupportedException();
        public override IBlock GetBlock<TPlatformModule>(TPlatformModule module, int? pageId) => throw new NotSupportedException();

        protected override IBlock GetBlock(IModule module, int? pageId) => throw new NotSupportedException();
    }
}
