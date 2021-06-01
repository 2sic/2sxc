using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Tests.PageProperty
{
    [TestClass]
    public class UpdatePropertyTest
    {
        protected const string Suffix = " - Blog - MySite";

        [TestMethod]
        public void PlaceholderSimple()
        {
            var result = Helpers.UpdateProperty("[placeholder]" + Suffix,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title" + Suffix, result);
        }

        [TestMethod]
        public void PlaceholderEnd()
        {
            var result = Helpers.UpdateProperty(Suffix + "[placeholder]",
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual(Suffix + "My Title", result);
        }

        [TestMethod]
        public void PlaceholderMiddle()
        {
            var result = Helpers.UpdateProperty("Before-[placeholder]-After",
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("Before-My Title-After", result);
        }

        [TestMethod]
        public void PlaceholderOtherCase()
        {
            var result = Helpers.UpdateProperty("[PlaceHolder]" + Suffix,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title" + Suffix, result);
        }

        [TestMethod]
        public void PlaceholderOnly()
        {
            var result = Helpers.UpdateProperty("[PlaceHolder]",
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void PlaceholderNotFound()
        {
            var result = Helpers.UpdateProperty(Suffix,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void PlaceholderNotFoundReplace()
        {
            var result = Helpers.UpdateProperty(Suffix,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Replace });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void PlaceholderNotFoundPrepend()
        {
            var result = Helpers.UpdateProperty(Suffix,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Prepend });
            Assert.AreEqual("My Title" + Suffix, result);
        }

        [TestMethod]
        public void PlaceholderNotFoundAppend()
        {
            var result = Helpers.UpdateProperty(Suffix,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Append });
            Assert.AreEqual(Suffix + "My Title", result);
        }
        [TestMethod]
        public void PlaceholderNotFoundAuto()
        {
            var result = Helpers.UpdateProperty(Suffix,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Auto });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void NullOriginal()
        {
            var result = Helpers.UpdateProperty(null,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
            Assert.AreEqual("My Title", result);
        }

        [TestMethod]
        public void ValueNull()
        {
            var result = Helpers.UpdateProperty("Some Title",
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = null });
            Assert.AreEqual("Some Title", result);
        }

        [TestMethod]
        public void AllNull()
        {
            var result = Helpers.UpdateProperty(null,
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = null });
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ValueEmpty()
        {
            var result = Helpers.UpdateProperty("Some Title",
                new PagePropertyChange { ReplacementIdentifier = "[placeholder]", Value = "" });
            Assert.AreEqual("", result);
        }
    }
}
