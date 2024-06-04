using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.Tests.DataTests
{
    [TestClass]
    public class GetAndConvertHelperTests
    {
        [TestMethod]
        public void GetFinalLanguagesList_LangsNull()
        {
            var dims = new[] { "en", "de", null };
            var (skipAddDef, languages) = GetAndConvertHelper.GetFinalLanguagesList(null, dims);
            Assert.IsFalse(skipAddDef);
            CollectionAssert.AreEqual(dims, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsEmpty()
        {
            var dims = new[] { "en", "de", null };
            var (skipAddDefault, languages) = GetAndConvertHelper.GetFinalLanguagesList("", dims);
            Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new string[] { null }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsFirst()
        {
            var dims = new[] { "en", "de", null };
            var (skipAddDefault, languages) = GetAndConvertHelper.GetFinalLanguagesList("en", dims);
            Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en" }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsBoth()
        {
            var dims = new[] { "en", "de", null };
            var (skipAddDefault, languages) = GetAndConvertHelper.GetFinalLanguagesList("en,de", dims);
            Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en", "de" }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsFirstAndEmpty()
        {
            var dims = new[] { "en", "de", null };
            var (skipAddDefault, languages) = GetAndConvertHelper.GetFinalLanguagesList("en,", dims);
            Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en", null }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsFirstShortened()
        {
            var dims = new[] { "en-us", "de-CH", null };
            var (skipAddDefault, languages) = GetAndConvertHelper.GetFinalLanguagesList("en", dims);
            Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(new[] { "en-us" }, languages);
        }

        [TestMethod]
        public void GetFinalLanguagesList_LangsNotFound()
        {
            var dims = new[] { "en-us", "de-CH", null };
            var (skipAddDefault, languages) = GetAndConvertHelper.GetFinalLanguagesList("qr", dims);
            Assert.IsTrue(skipAddDefault);
            CollectionAssert.AreEqual(Array.Empty<string>() , languages);
        }

    }
}
