using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Data.Build;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Internal.LightSpeed;
using ToSic.Sxc.Web.Internal.Url;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebLightSpeed
{
    [TestClass]
    public class LightSpeedUrlParamsTest: TestBaseEavCore
    {
        public LightSpeedUrlParamsTest() => _testData = new(GetService<DataBuilder>());

        private readonly LightSpeedTestData _testData;

        private NameValueCollection Parse(string query) => UrlHelpers.ParseQueryString(query);



        [DataRow(true, null, "no setting")]
        [DataRow(true, true, "true")]
        [DataRow(false, false, "false")]
        [TestMethod]
        public void IsEnabled(bool expected, bool? isEnabled, string message)
        {
            var lsDecorator = _testData.Decorator(isEnabled: isEnabled);
            var result = LightSpeedUrlParams.GetUrlParams(lsDecorator, new Parameters(), null);
            AreEqual(expected, result.CachingAllowed, message);
            AreEqual("", result.Extension);
        }

        [DataRow(true, "", null, null, "")]
        [DataRow(true, "", true, true, "")]
        [DataRow(true, "", true, false, "")]
        [DataRow(true, "", true, false, "a=b")]
        [DataRow(true, "a=b", true, true, "a=b")]
        [DataRow(true, "a=b&test=y", true, true, "test=y&a=b", DisplayName = "Ensure parameters are sorted")]
        [DataRow(true, "a=b&test=y&zeta=beta", true, true, "zeta=beta&test=y&a=b", DisplayName = "Ensure parameters are sorted")]
        [DataRow(true, "a=alpha&a=beta", true, true, "a=beta&a=alpha", DisplayName = "Ensure parameter values are sorted")]
        [DataRow(false, "", false, null, "")]
        [TestMethod]
        public void ByUrlParameters(bool expected, string expValue, bool? isEnabled, bool? byUrlParameters, string urlParameters, string message = default)
        {
            var lsDecorator = _testData.Decorator(isEnabled: isEnabled, byUrlParameters: byUrlParameters);
            var result = LightSpeedUrlParams.GetUrlParams(lsDecorator, new Parameters(Parse(urlParameters)), null);
            AreEqual(expected, result.CachingAllowed, message);
            AreEqual(expValue, result.Extension);
        }

        [DataRow("", "", "")]
        [DataRow("", "a", "")]
        [DataRow("a=b", "a", "a=b")]
        [DataRow("a=b&b=c", "a,b", "a=b&b=c")]
        [DataRow("a=b", "a,b", "a=b", DisplayName = "more possible, but only some given")]
        [DataRow("a=b", "*", "a=b")]
        [DataRow("a=b&c=d", "*", "a=b&c=d")]
        [TestMethod]
        public void NamesShouldFilterResults(string expValue, string names, string urlParameters, string message = default)
        {
            var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: names, othersDisableCache: true);
            var result = LightSpeedUrlParams.GetUrlParams(lsDecorator, new Parameters(Parse(urlParameters)), null);
            AreEqual(true, result.CachingAllowed, message);
            AreEqual(expValue, result.Extension);
        }

        [DataRow(true, "", "", DisplayName = "no names, params")]
        [DataRow(false, "", "a=b", DisplayName = "has params but none expected")]
        [DataRow(true, "a", "a=b", DisplayName = "Has params, expected")]
        [DataRow(false, "b", "a=b", DisplayName = "Has params, others expected")]
        [DataRow(true, "a,b", "a=b&b=c", DisplayName = "Has params, expected")]
        [DataRow(false, "a", "a=b&b=c", DisplayName = "Has params, not all expected")]
        [DataRow(true, "*", "a=b", DisplayName = "has params, all expected")]
        [DataRow(true, "*", "a=b&b=c", DisplayName = "has params, all expected")]
        [DataRow(true, "a,b", "a=b", DisplayName = "Has params, more can be expected")]
        [DataRow(true, "a,b", "", DisplayName = "No params, various expected")]
        [TestMethod]
        public void NamesShouldDisable(bool expected, string names, string urlParameters)
        {
            var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: names, othersDisableCache: true);
            var result = LightSpeedUrlParams.GetUrlParams(lsDecorator, new Parameters(Parse(urlParameters)), null);
            AreEqual(expected, result.CachingAllowed);
        }

        [DataRow("", "", "")]
        [DataRow("", "a", "")]
        [DataRow("a=b", "a // this is because xyz", "a=b")]
        [DataRow("a=b&b=c", "a // ok\nb // also ok", "a=b&b=c")]
        [DataRow("a=b&c=d", "* // whatever", "a=b&c=d")]
        [TestMethod]
        public void NamesCanBeMultilineAndCommented(string expValue, string names, string urlParameters, string message = default)
        {
            var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: names, othersDisableCache: true);
            var result = LightSpeedUrlParams.GetUrlParams(lsDecorator, new Parameters(Parse(urlParameters)), null);
            AreEqual(true, result.CachingAllowed, message);
            AreEqual(expValue, result.Extension);
        }


        [TestMethod]
        public void LoadTestWithoutCaching()
        {
            const int repeat = 10000;
            var lsDecorator = _testData.Decorator(isEnabled: true, byUrlParameters: true, names: "a // nice idea\nb // ok too, but a bit nasty\n\ntest // another one to parse", othersDisableCache: true);
            var parameters = new Parameters(Parse("ZETA=last&d=27&a=b&b=c"));

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < repeat; i++) 
                LightSpeedUrlParams.GetUrlParams(lsDecorator, parameters, null, usePiggyBack: false);
            stopwatch.Stop();

            Trace.WriteLine($"Ran {repeat} iterations without cache, duration was {stopwatch.ElapsedMilliseconds}ms");

            stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < repeat; i++) 
                LightSpeedUrlParams.GetUrlParams(lsDecorator, parameters, null, usePiggyBack: true);
            stopwatch.Stop();

            Trace.WriteLine($"Ran {repeat} iterations with cache, duration was {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
