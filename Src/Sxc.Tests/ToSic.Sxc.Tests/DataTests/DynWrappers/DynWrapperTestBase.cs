using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    public class DynWrapperTestBase: TestBaseSxcDb
    {
        public DynamicWrapperFactory Factory => _wrapFac.Get(GetService<DynamicWrapperFactory>);
        private readonly GetOnce<DynamicWrapperFactory> _wrapFac = new GetOnce<DynamicWrapperFactory>();

        public DynamicReadObject DynFromObject(object data, bool wrapChildren, bool realObjectsToo)
            => Factory.FromObject(data, ReWrapSettings.Dyn(children: wrapChildren, realObjectsToo: realObjectsToo));

        public ITyped TypedFromObject(object data, ReWrapSettings reWrap = null)
            => Factory.TypedFromObject(data, reWrap ?? ReWrapSettings.Typed(true, true));
    }
}
