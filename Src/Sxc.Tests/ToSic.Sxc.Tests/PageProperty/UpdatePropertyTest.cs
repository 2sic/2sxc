using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Tests.PageProperty
{
    [TestClass]
    public class UpdatePropertyTest
    {
        protected const string Suffix = " - Blog - MySite";

        private string UpdatePropertyTestAccessor(string original, PagePropertyChange change) =>
            Helpers.UpdateProperty(original, change);

        [TestMethod]
        public void PlaceholderSimple()
        {
            var result = UpdatePropertyTestAccessor("[placeholder]" + Suffix,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title" + Suffix, result);
        }

        [TestMethod]
        public void PlaceholderEnd()
        {
            var result = UpdatePropertyTestAccessor(Suffix + "[placeholder]",
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual(Suffix + "My Title", result);
        }

        [TestMethod]
        public void PlaceholderMiddle()
        {
            var result = UpdatePropertyTestAccessor("Before-[placeholder]-After",
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("Before-My Title-After", result);
        }

        [TestMethod]
        public void PlaceholderOtherCase()
        {
            var result = UpdatePropertyTestAccessor("[PlaceHolder]" + Suffix,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title" + Suffix, result);
        }

        [TestMethod]
        public void PlaceholderOnly()
        {
            var result = UpdatePropertyTestAccessor("[PlaceHolder]",
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void PlaceholderNotFound()
        {
            var result = UpdatePropertyTestAccessor(Suffix,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void PlaceholderNotFoundReplace()
        {
            var result = UpdatePropertyTestAccessor(Suffix,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Replace });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void PlaceholderNotFoundPrepend()
        {
            var result = UpdatePropertyTestAccessor(Suffix,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Prepend });
            Assert.AreEqual("My Title" + Suffix, result);
        }

        [TestMethod]
        public void PlaceholderNotFoundAppend()
        {
            var result = UpdatePropertyTestAccessor(Suffix,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Append });
            Assert.AreEqual(Suffix + "My Title", result);
        }
        [TestMethod]
        public void PlaceholderNotFoundAuto()
        {
            var result = UpdatePropertyTestAccessor(Suffix,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Auto });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void NullOriginal()
        {
            var result = UpdatePropertyTestAccessor(null,
                new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void ValueNull()
        {
            var result = UpdatePropertyTestAccessor("Some Title",
                new() { ReplacementIdentifier = "[placeholder]", Value = null });
            Assert.AreEqual("Some Title", result);
        }

        [TestMethod]
        public void AllNull()
        {
            var result = UpdatePropertyTestAccessor(null,
                new() { ReplacementIdentifier = "[placeholder]", Value = null });
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ValueEmpty()
        {
            var result = UpdatePropertyTestAccessor("Some Title",
                new() { ReplacementIdentifier = "[placeholder]", Value = "" });
            Assert.AreEqual("", result);
        }
    }
}
