using System;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    public abstract class ModuleAndBlockBuilder: HasLog<IModuleAndBlockBuilder>, IModuleAndBlockBuilder
    {
        protected ModuleAndBlockBuilder(string logPrefix): base($"{logPrefix}.BnMBld")
        {
        }

        protected abstract IModule GetModuleImplementation(int pageId, int moduleId);

        protected void ThrowIfModuleIsNull<TModule>(int pageId, int moduleId, TModule moduleInfo)
        {
            if (moduleInfo != null) return;
            var msg = $"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?";
            Log.A(msg);
            throw new Exception(msg);
        }

        public IBlock GetBlock(int pageId, int moduleId)
        {
            var wrapLog = Log.Fn<IBlock>($"{pageId}, {moduleId}");
            var module = GetModuleImplementation(pageId, moduleId);
            var result = GetBlock(module, pageId);
            return wrapLog.ReturnAsOk(result);
        }

        public abstract IBlock GetBlock<TPlatformModule>(TPlatformModule module, int? pageId) where TPlatformModule : class;

        protected abstract IBlock GetBlock(IModule module, int? pageId);
    }
}
