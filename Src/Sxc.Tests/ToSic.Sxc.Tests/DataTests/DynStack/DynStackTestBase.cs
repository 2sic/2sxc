using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;

namespace ToSic.Sxc.Tests.DataTests.DynStack
{
    public abstract class DynStackTestBase : TestBaseSxcDb
    {
        public CodeDataWrapper Wrapper => _wrapFac.Get(GetService<CodeDataWrapper>);
        private readonly GetOnce<CodeDataWrapper> _wrapFac = new GetOnce<CodeDataWrapper>();
        public CodeDataFactory Factory => _fac.Get(GetService<CodeDataFactory>);
        private readonly GetOnce<CodeDataFactory> _fac = new GetOnce<CodeDataFactory>();

        public ITyped TypedFromObject(object data, WrapperSettings? reWrap = null)
            => Wrapper.TypedFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

    }
}
