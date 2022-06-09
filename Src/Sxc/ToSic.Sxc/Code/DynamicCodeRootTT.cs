using ToSic.Eav.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public abstract class DynamicCodeRoot<TModel, TKit>: DynamicCodeRoot, IDynamicCodeRoot<TModel, TKit>
        where TModel : class
        where TKit : KitBase
    {
        protected DynamicCodeRoot(Dependencies dependencies, string logPrefix) : base(dependencies, logPrefix)
        {
        }

        public TModel Model => default; // _mOnce.Get(GetService<TModel>);
        //private readonly ValueGetOnce<TModel> _mOnce = new ValueGetOnce<TModel>();

        public TKit Kit => _kit.Get(GetService<TKit>);
        private readonly ValueGetOnce<TKit> _kit = new ValueGetOnce<TKit>();
    }
}
