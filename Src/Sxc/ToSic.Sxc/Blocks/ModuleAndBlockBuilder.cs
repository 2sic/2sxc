using System;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks
{
    public abstract class ModuleAndBlockBuilder: HasLog<IModuleAndBlockBuilder>, IModuleAndBlockBuilder
    {
        protected ModuleAndBlockBuilder(string logPrefix): base($"{logPrefix}.BnMBld")
        {
        }

        // 2022-02-16 2dm - ATM not needed, maybe we'll reactivate if ever requested
        //public IModule GetModule(int pageId, int moduleId)
        //{
        //    var wrapLog = Log.Call<IModule>($"{pageId}, {moduleId}");
        //    return wrapLog("ok", GetModuleImplementation(pageId, moduleId));
        //}
        //public IBlock GetBlock(IModule module)
        //{
        //    var wrapLog = Log.Call<IBlock>($"module: {module?.Id}");
        //    ThrowIfModuleIsNull(module);
        //    return wrapLog("ok", GetBlockImplementation(module));
        //}
        //protected void ThrowIfModuleIsNull<TModule>(TModule moduleInfo)
        //{
        //    if (moduleInfo != null) return;
        //    var msg = $"Module is Null. Can't continue.";
        //    Log.A(msg);
        //    throw new Exception(msg);
        //}

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
            var result = GetBlock(module);
            return wrapLog.Return(result, "ok");
        }

        public abstract IBlock GetBlock<TPlatformModule>(TPlatformModule module) where TPlatformModule : class;

        protected abstract IBlock GetBlock(IModule module);
    }
}
