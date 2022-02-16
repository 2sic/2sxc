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

        public IModule GetModule(int pageId, int moduleId)
        {
            var wrapLog = Log.Call<IModule>($"{pageId}, {moduleId}");
            return wrapLog("ok", GetModuleImplementation(pageId, moduleId));
        }

        protected abstract IModule GetModuleImplementation(int pageId, int moduleId);

        protected void ThrowIfModuleIsNull<TModule>(int pageId, int moduleId, TModule moduleInfo)
        {
            if (moduleInfo != null) return;
            var msg = $"Can't find module {moduleId} on page {pageId}. Maybe you reversed the ID-order?";
            Log.Add(msg);
            throw new Exception(msg);
        }

        public IBlockBuilder GetBuilder(int pageId, int moduleId)
        {
            var wrapLog = Log.Call<IBlockBuilder>($"{pageId}, {moduleId}");
            return wrapLog("ok", GetBuilderImplementation(pageId, moduleId));

        }
        protected abstract IBlockBuilder GetBuilderImplementation(int pageId, int moduleId);

    }
}
