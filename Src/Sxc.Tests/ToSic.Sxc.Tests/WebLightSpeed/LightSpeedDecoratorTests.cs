using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data.Build;
using ToSic.Sxc.Web.Internal.LightSpeed;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebLightSpeed
{
    [TestClass]
    public class LightSpeedDecoratorTests: TestBaseEavCore
    {
        public LightSpeedDecoratorTests() => _testData = new(GetService<DataBuilder>());

        private readonly LightSpeedTestData _testData;


        [TestMethod]
        public void DecoratorWithNullEntity()
        {
            var lsDecorator = new LightSpeedDecorator(null);
            TestEmptyDecorator(lsDecorator);
            AreEqual(true, lsDecorator.UrlParametersOthersDisableCache);
        }

        [TestMethod]
        public void DecoratorWithEntity()
        {
            var lsDecorator = _testData.Decorator();
            AreEqual(LightSpeedTestData.DefTitle, lsDecorator.Title);
            TestEmptyDecorator(lsDecorator);
        }

        private static void TestEmptyDecorator(LightSpeedDecorator lsDecorator)
        {
            AreEqual(false, lsDecorator.IsEnabled);
            AreEqual("", lsDecorator.UrlParameterNames);
            AreEqual(false, lsDecorator.ByUrlParameters);
            AreEqual(false, lsDecorator.UrlParametersCaseSensitive);
            AreEqual(true, lsDecorator.UrlParametersOthersDisableCache);
        }

        [TestMethod]
        public void DecoratorWithByUrlParameter()
            => AreEqual(true, _testData.Decorator(byUrlParameters: true).ByUrlParameters);

        [TestMethod]
        public void DecoratorWithCaseSensitive()
            => AreEqual(true, _testData.Decorator(caseSensitive: true).UrlParametersCaseSensitive);

        [TestMethod]
        public void DecoratorWithNames()
            => AreEqual("a\nb\nc", _testData.Decorator(names: "a\nb\nc").UrlParameterNames);

        [TestMethod]
        public void DecoratorWithDisableCache()
            => AreEqual(false, _testData.Decorator(othersDisableCache: false).UrlParametersOthersDisableCache);

    }
}
