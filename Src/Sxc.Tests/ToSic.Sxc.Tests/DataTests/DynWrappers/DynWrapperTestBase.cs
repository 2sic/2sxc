using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    public class DynWrapperTestBase: TestBaseSxcDb
    {
        public DynamicWrapperFactory Factory => _wrapFac.Get(GetService<DynamicWrapperFactory>);
        private readonly GetOnce<DynamicWrapperFactory> _wrapFac = new GetOnce<DynamicWrapperFactory>();

        public DynamicReadObject DynFromObject(object data, bool wrapChildren, bool wrapRealChildren)
            => Factory.FromObject(data, wrapChildren, wrapRealChildren);

        public ITyped TypedFromObject(object data, bool wrapChildren, bool wrapRealChildren)
            => Factory.TypedFromObject(data, wrapChildren, wrapRealChildren);
    }
}
