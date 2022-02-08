using System;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// WIP - try to make an injectable Context Initializer
    /// So it can be implemented the same way in Dnn/Oqtane
    /// </summary>
    public abstract class ContextOfBlockFactoryWip: HasLog<ContextOfBlockFactoryWip>
    {
        protected ContextOfBlockFactoryWip(IContextOfBlock context, string logPrefix): base($"{logPrefix}.CtxBlF")
        {
            Context = context;
        }
        protected readonly IContextOfBlock Context;
        protected bool AlreadyConfigured = false;

        public IContextOfBlock Configure(IModule module)
        {
            var wrapLog = Log.Call<IContextOfBlock>($"{module?.Id}");
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (AlreadyConfigured) throw new Exception($"{nameof(Configure)} can only be called once. Then you need a new service.");
            var configured = ConfigureImplementation(module);
            AlreadyConfigured = true;
            return wrapLog("ok", configured);
        }

        protected abstract IContextOfBlock ConfigureImplementation(IModule module);

    }
}
