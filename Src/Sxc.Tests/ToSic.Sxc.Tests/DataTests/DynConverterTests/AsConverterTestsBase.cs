using ToSic.Sxc.Data;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    public class AsConverterTestsBase : TestBaseSxcDb
    {
        public CodeDataFactory Cdf => _cdf ??= GetService<CodeDataFactory>();
        private CodeDataFactory _cdf;
    }
}
