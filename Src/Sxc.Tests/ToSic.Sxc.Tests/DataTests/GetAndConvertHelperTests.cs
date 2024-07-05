using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static ToSic.Sxc.Tests.DataTests.GetAndConvertHelperTestAccessors;

namespace ToSic.Sxc.Tests.DataTests
{
    [TestClass]
    public class GetAndConvertHelperTests
    {
        private static readonly List<string> MockSystemLanguages2 = ["en", "de"];
        private static readonly List<string> MockSystemLanguages4 = ["en-us", "de-ch", "fr-fr"];

        [TestMethod]
        public void GetFinalLanguagesList_LangsNull()
        {
            var dims = new[] { "en", "de", null };
            var languages = TacGetFinalLanguagesList(null, MockSystemLanguages2, dims);
            //Assert.IsTrue(skipAddDef);
            CollectionAssert.AreEqual(dims, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsEmpty()
        {
            var dims = new[] { "en", "de", null };
            var languages = TacGetFinalLanguagesList("", MockSystemLanguages2, dims);
            //Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new string[] { null }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsFirst()
        {
            var dims = new[] { "en", "de", null };
            var languages = TacGetFinalLanguagesList("en", MockSystemLanguages2, dims);
            //Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en" }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsBoth()
        {
            var dims = new[] { "en", "de", null };
            var languages = TacGetFinalLanguagesList("en,de", MockSystemLanguages2, dims);
            //Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en", "de" }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsFirstAndEmpty()
        {
            var dims = new[] { "en", "de", null };
            var languages = TacGetFinalLanguagesList("en,", MockSystemLanguages2, dims);
            //Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en", null }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsFirstShortened()
        {
            var dims = new[] { "en-us", "de-CH", null };
            var languages = TacGetFinalLanguagesList("en", MockSystemLanguages4, dims);
            //Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en-us" }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsNotFound()
        {
            var dims = new[] { "en-us", "de-CH", null };
            var languages = TacGetFinalLanguagesList("qr", MockSystemLanguages4, dims);
            //Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(Array.Empty<string>() , languages);
        }

    }
}
