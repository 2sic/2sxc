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

        protected override IModule GetModuleImplementation(int pageId, int moduleId) => throw new NotImplementedException();

        protected override IBlockBuilder GetBuilderImplementation(int pageId, int moduleId) => throw new NotImplementedException();
    }
}
