using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages
{
    [TestClass]
    public class QueryHelperTests
    {
        private static string AddQueryString(string url, NameValueCollection queryParams)
            => ToSic.Sxc.Web.Internal.Url.UrlHelpers.AddQueryString(url, queryParams);

        private const string urlRelativeNoParams = "/xyz/abc.jpg";
        private const string urlHttpNoParams = "http://2sxc.org/xyz/abc.jpg";
        private const string urlHttpsNoParams = "https://2sxc.org/xyz/abc.jpg";
        private const string paramOne = "?param=value";
        private const string paramTwo = paramOne + "&name=iJungleboy";
        public const string urlFile27 = "file:27";
        public const string urlPage42 = "page:42";
        public const string fragEmpty = "#";
        public const string fragValueOnly = "#someFragment";
        public const string fragKeyValue = "#some=fragMEnt";
        public const string fragKeyValueMany = "#Some=fraGMent&othr=OTHR";
        public const string fragWithQuestion = "#some=how-are-you?";

        private const NameValueCollection addOnNull = null;
        private static NameValueCollection addOnEmpty = new();
        private static NameValueCollection addOnOne = new()
        {
            {"added", "worked"}
        };

        private string paramAddOnOne = "added=worked";

        [TestMethod]
        public void EmptyUrls()
        {
            VerifyUnchangedWithoutAddOn(null);
            VerifyUnchangedWithoutAddOn(string.Empty);
            VerifyUnchangedWithoutAddOn(" ");
        }

        [TestMethod]
        public void FileReferenceUrls()
        {
            VerifyUnchangedWithoutAddOn("file:27");
            VerifyUnchangedWithoutAddOn("page:42030");
            VerifyUnchangedWithoutAddOn("file:305030?test=24");
            VerifyUnchangedWithoutAddOn(urlFile27 + fragEmpty);
            VerifyUnchangedWithoutAddOn(urlPage42 + fragEmpty);
            VerifyUnchangedWithoutAddOn(urlFile27 + fragValueOnly);
            VerifyUnchangedWithoutAddOn(urlPage42 + fragValueOnly);
            VerifyUnchangedWithoutAddOn(urlFile27 + fragKeyValue);
            VerifyUnchangedWithoutAddOn(urlFile27 + fragKeyValueMany);
            VerifyUnchangedWithoutAddOn(urlFile27 + fragWithQuestion);
        }
        [TestMethod]
        public void FileReferenceUrlsWithAddOns()
        {

            AreEqual(urlFile27 + "?" + paramAddOnOne, AddQueryString(urlFile27, addOnOne));
            AreEqual(urlPage42 + "?" + paramAddOnOne, AddQueryString(urlPage42, addOnOne));
            AreEqual(urlPage42 + "?" + paramAddOnOne, AddQueryString(urlPage42, addOnOne));

            var urlFWithParam = urlFile27 + paramOne;
            AreEqual(urlFWithParam + "&" + paramAddOnOne, AddQueryString(urlFWithParam, addOnOne));

            // With fragment and with empty fragment
            AreEqual(urlFile27+ "?" + paramAddOnOne, AddQueryString(urlFile27 + fragEmpty, addOnOne), "empty frag is dropped");
            AreEqual(urlFile27 + "?" + paramAddOnOne + fragValueOnly, AddQueryString(urlFile27 + fragValueOnly, addOnOne));
            AreEqual(urlFile27 + "?" + paramAddOnOne + fragKeyValue, AddQueryString(urlFile27 + fragKeyValue, addOnOne));
            AreEqual(urlFile27 + "?" + paramAddOnOne + fragKeyValueMany, AddQueryString(urlFile27 + fragKeyValueMany, addOnOne));
            AreEqual(urlFile27 + "?" + paramAddOnOne + fragWithQuestion, AddQueryString(urlFile27 + fragWithQuestion, addOnOne));
        }

        [TestMethod]
        public void EmptyUrlWithExistingParams()
        {
            VerifyUnchangedWithoutAddOn(paramOne);
            VerifyUnchangedWithoutAddOn(paramOne + paramTwo);
            VerifyUnchangedWithoutAddOn(" " + paramOne);
        }

        [TestMethod]
        public void NoParams()
        {
            VerifyUnchangedWithoutAddOn(urlRelativeNoParams);
            VerifyUnchangedWithoutAddOn(urlHttpNoParams);
            VerifyUnchangedWithoutAddOn(urlHttpsNoParams);
        }

        [TestMethod]
        public void ParamsButNoAdditional()
        {
            VerifyUnchangedWithoutAddOn(urlRelativeNoParams + paramOne);
            VerifyUnchangedWithoutAddOn(urlHttpNoParams + paramOne);
            VerifyUnchangedWithoutAddOn(urlHttpsNoParams + paramOne);
            VerifyUnchangedWithoutAddOn(urlRelativeNoParams + paramOne + paramTwo);
            VerifyUnchangedWithoutAddOn(urlHttpNoParams + paramOne + paramTwo);
            VerifyUnchangedWithoutAddOn(urlHttpsNoParams + paramOne + paramTwo);
        }

        [TestMethod]
        public void NoParamsButAddOne()
        {
            AreEqual(urlRelativeNoParams + "?" + paramAddOnOne, AddQueryString(urlRelativeNoParams, addOnOne));
            AreEqual(urlHttpNoParams + "?" + paramAddOnOne, AddQueryString(urlHttpNoParams, addOnOne));
            AreEqual(urlHttpsNoParams+ "?" + paramAddOnOne, AddQueryString(urlHttpsNoParams, addOnOne));
        }

        [TestMethod]
        public void ParamsAndAddOne()
        {
            AreEqual($"{urlRelativeNoParams}{paramOne}&{paramAddOnOne}", AddQueryString($"{urlRelativeNoParams}{paramOne}", addOnOne));
            AreEqual($"{urlHttpNoParams}{paramOne}&{paramAddOnOne}", AddQueryString($"{urlHttpNoParams}{paramOne}", addOnOne));
            AreEqual($"{urlHttpsNoParams}{paramOne}&{paramAddOnOne}", AddQueryString($"{urlHttpsNoParams}{paramOne}", addOnOne));
        }



        private void VerifyUnchangedWithoutAddOn(string url)
        {
            AreEqual(url, AddQueryString(url , addOnNull), $"Initial: {url}");
            AreEqual(url, AddQueryString(url , addOnEmpty), $"Initial {url}");
        }

    }
}
