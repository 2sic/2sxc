using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static ToSic.Sxc.Services.Internal.LinkServiceUnknown;
using static ToSic.Sxc.Tests.LinksAndImages.ParametersTestExtensions;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToPartTests: LinkHelperToTestBase
    {

        [TestMethod]
        public void NoPage()
        {
            TestToPageParts(null, null, NiceCurrentUrl, NiceCurrentUrl, NiceCurrentRelative);
        }

        [TestMethod]
        public void NoPageStringParamWithQuestion()
        {
            var query = "active=true";
            var exp = $"{NiceCurrentUrlRoot}?{query}";
            var rel = $"{NiceCurrentRelative}?{query}";
            TestToPageParts(null, $"?{query}", exp, exp, rel);
        }

        [TestMethod]
        public void NoPageStringParamWithAmpersand()
        {
            var query = "active=true";
            var exp = $"{NiceCurrentUrlRoot}?{query}";
            var rel = $"{NiceCurrentRelative}?{query}";
            TestToPageParts(null, $"&{query}", exp, exp, rel);
        }

        private void NoPageStringParam(string query)
        {
            var exp = NiceCurrentUrlRoot + "?" + query;
            var rel = $"{NiceCurrentRelative}?{query}";
            TestToPageParts(null, query, exp, exp, rel);
        }

        [TestMethod] public void NoPageStringParamNoValue() => NoPageStringParam("active");
        [TestMethod] public void NoPageStringParamNoValue2() => NoPageStringParam("active&passive");
        [TestMethod] public void NoPageStringParamValue1() => NoPageStringParam("this=active");
        [TestMethod] public void NoPageStringParamValue2() => NoPageStringParam("this=active&that=passive");
        [TestMethod] public void NoPageStringParamValueParamNoValue() => NoPageStringParam("this=active&passive");
        [TestMethod] public void NoPageStringParamNoValueParamValue() => NoPageStringParam("active&that=passive");

        private void NoPageObjectParam(object parameters, string expQuery)
        {
            var fullQuery = (string.IsNullOrEmpty(expQuery) ? "" : "?") + expQuery;
            var exp = NiceCurrentUrlRoot + fullQuery;
            var rel = $"{NiceCurrentRelative}{fullQuery}";
            TestToPageParts(null, parameters, exp, exp, rel);
        }

        [TestMethod] public void NoPageObjectParamUnsupported() => NoPageObjectParam(new DateTime(), "");

        [TestMethod]
        public void NoPageObjectParamsEmpty() => NoPageObjectParam(NewParameters(null), "");

        [TestMethod]
        public void NoPageObjectParamsKeyOnly() =>
            NoPageObjectParam(NewParameters(new() { { "active", null } }), "active");

        [TestMethod]
        public void NoPageObjectParamsKeyValueEmpty() =>
            NoPageObjectParam(NewParameters(new() { { "active", "" } }), "active");

        [TestMethod]
        public void NoPageObjectParamsKeyEmpty() =>
            NoPageObjectParam(NewParameters(new() { { "", "" } }), "");

        [TestMethod]
        public void NoPageObjectParamDicKeyValue() =>
            NoPageObjectParam(NewParameters(new() { { "active", "true" } }), "active=true");

        [TestMethod]
        public void NoPageObjectParamDicObject() =>
            NoPageObjectParam(NewParameters(new() { { "active", "true" }, {"passive", "false"} }), "active=true&passive=false");


        /// <summary>
        /// This will reconfigure the LinkHelperUnknown to deliver ugly dnn-url like ...?tabId=27
        /// </summary>
        /// <param name="action"></param>
        private void RunWithUglyUrl(Action action)
        {
            SwitchModeToUgly(true);
            try
            {
                action.Invoke();
            }
            finally
            {
                SwitchModeToUgly(false);
            }
        }

        //[TestMethod]
        //[Ignore]
        //public void NoPageUgly()
        //{
        //    RunWithUglyUrl(() => { }
        //        //TestToPageParts(null, null, UglyCurrentUrl, UglyCurrentUrl, DefProtocol, DefDomain, null, null,
        //        //    UglyCurrentQuery, "", "?" + UglyCurrentQuery)
        //    );
        //}


        [TestMethod]
        public void Page27Plain()
        {
            var exp = NiceAnyPageUrl.Replace("{0}", "27");
            var rel = NiceAnyRelative.Replace("{0}", "27");
            TestToPageParts(27, null, exp, exp, rel);
        }

        private void Page27StringParam(string query)
        {
            var exp = NiceCurrentUrlRoot + "?" + query;
            var rel = NiceAnyRelative.Replace("{0}", "27") + "?" + query;
            TestToPageParts(27, query, exp, exp, rel);
        }
    }
}
