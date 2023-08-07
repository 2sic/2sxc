using ToSic.Sxc.Data.AsConverter;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests
{
    public class AsConverterTestsBase : TestBaseSxcDb
    {
        public AsConverterService AsC => _asc ?? (_asc = GetService<AsConverterService>());
        private AsConverterService _asc;

    }
}
