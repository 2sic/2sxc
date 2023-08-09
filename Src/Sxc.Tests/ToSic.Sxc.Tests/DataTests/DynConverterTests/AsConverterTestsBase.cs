using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    public class AsConverterTestsBase : TestBaseSxcDb
    {
        public CodeDataFactory AsC => _asc ?? (_asc = GetService<CodeDataFactory>());
        private CodeDataFactory _asc;
    }
}
