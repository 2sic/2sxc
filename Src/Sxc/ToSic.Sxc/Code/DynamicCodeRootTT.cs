using ToSic.Lib.Helpers;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public abstract class DynamicCodeRoot<TModel, TServiceKit>: DynamicCodeRoot, IDynamicCodeRoot<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        protected DynamicCodeRoot(MyServices services, string logPrefix) : base(services, logPrefix)
        {
        }

        public TModel Model => default;

        public TServiceKit Kit => _kit.Get(GetService<TServiceKit>);
        private readonly GetOnce<TServiceKit> _kit = new GetOnce<TServiceKit>();
    }
}
