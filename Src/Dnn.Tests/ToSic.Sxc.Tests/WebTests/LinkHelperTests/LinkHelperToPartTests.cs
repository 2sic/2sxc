using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Context.Query;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static ToSic.Sxc.Web.LinkHelperUnknown;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass]
    public class LinkHelperToPartTests: LinkHelperToTestBase
    {

        [TestMethod]
        public void NoPage()
        {
            TestToPageParts(null, null, NiceCurrentUrl, NiceCurrentUrl, DefProtocol, DefDomain, null, null, "", "","");
        }

        [TestMethod]
        public void NoPageStringParamWithQuestion()
        {
            var query = "active=true";
            var exp = $"{NiceCurrentUrlRoot}?{query}";
            TestToPageParts(null, $"?{query}", exp, exp, DefProtocol, DefDomain, null, null, query, string.Empty,$"?{query}");
        }

        [TestMethod]
        public void NoPageStringParamWithAmpersand()
        {
            var query = "active=true";
            var exp = $"{NiceCurrentUrlRoot}?{query}";
            TestToPageParts(null, $"&{query}", exp, exp, DefProtocol, DefDomain, null, null, query, string.Empty,$"?{query}");
        }

        private void NoPageStringParam(string query)
        {
            var exp = NiceCurrentUrlRoot + "?" + query;
            TestToPageParts(null, query, exp, exp, DefProtocol, DefDomain, null, null, query, string.Empty, $"?{query}");
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
            TestToPageParts(null, parameters, exp, exp, DefProtocol, DefDomain, null, null, expQuery, string.Empty, fullQuery);
        }

        [TestMethod] public void NoPageObjectParamUnsupported() => NoPageObjectParam(new DateTime(), "");

        [TestMethod]
        public void NoPageObjectParamsEmpty() => NoPageObjectParam(new Parameters(null), "");

        [TestMethod]
        public void NoPageObjectParamsKeyOnly() =>
            NoPageObjectParam(new Parameters(new NameValueCollection { { "active", null } }), "active");

        [TestMethod]
        public void NoPageObjectParamsKeyValueEmpty() =>
            NoPageObjectParam(new Parameters(new NameValueCollection { { "active", "" } }), "active");

        [TestMethod]
        public void NoPageObjectParamsKeyEmpty() =>
            NoPageObjectParam(new Parameters(new NameValueCollection { { "", "" } }), "");

        [TestMethod]
        public void NoPageObjectParamDicKeyValue() =>
            NoPageObjectParam(new Parameters(new NameValueCollection { { "active", "true" } }), "active=true");

        [TestMethod]
        public void NoPageObjectParamDicObject() =>
            NoPageObjectParam(new Parameters(new NameValueCollection { { "active", "true" }, {"passive", "false"} }), "active=true&passive=false");


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

        [TestMethod]
        public void NoPageUgly()
        {
            RunWithUglyUrl(() =>
                TestToPageParts(null, null, UglyCurrentUrl, UglyCurrentUrl, DefProtocol, DefDomain, null, null,
                    UglyCurrentQuery, "", "?" + UglyCurrentQuery)
            );
        }


        [TestMethod]
        public void Page27Plain()
        {
            var exp = NiceAnyPageUrl.Replace("{0}", "27");
            TestToPageParts(27, null, exp, exp, DefProtocol, DefDomain, null, null, string.Empty, string.Empty, string.Empty);
        }

        private void Page27StringParam(string query)
        {
            var exp = NiceCurrentUrlRoot + "?" + query;
            TestToPageParts(27, query, exp, exp, DefProtocol, DefDomain, null, null, query, string.Empty,$"?{query}");
        }
    }
}
