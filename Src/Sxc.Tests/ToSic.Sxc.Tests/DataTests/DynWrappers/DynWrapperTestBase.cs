using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    public class DynWrapperTestBase: TestBaseSxcDb
    {
        public DynamicWrapperFactory Factory => _wrapFac.Get(GetService<DynamicWrapperFactory>);
        private readonly GetOnce<DynamicWrapperFactory> _wrapFac = new GetOnce<DynamicWrapperFactory>();

        public DynamicReadObject DynFromObject(object data, bool wrapChildren = true, bool realObjectsToo = true)
            => Factory.FromObject(data, WrapperSettings.Dyn(children: wrapChildren, realObjectsToo: realObjectsToo));

        public dynamic AsDynamic(object data) => DynFromObject(data);

        public ITyped TypedFromObject(object data, WrapperSettings? reWrap = null)
            => Factory.TypedFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

        public ITypedItem ItemFromObject(object data, WrapperSettings? reWrap = null)
            => Factory.TypedItemFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));
    }
}
