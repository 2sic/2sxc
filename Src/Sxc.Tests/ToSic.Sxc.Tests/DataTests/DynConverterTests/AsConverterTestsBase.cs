using ToSic.Sxc.Data;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    public class AsConverterTestsBase : TestBaseSxcDb
    {
        public CodeDataFactory Cdf => _cdf ??= GetService<CodeDataFactory>();
        private CodeDataFactory _cdf;
    }
}
