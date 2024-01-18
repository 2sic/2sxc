using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Internal.Url;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests
{
    [TestClass]
    public class Obj2UrlMerge
    {
        // Test accessor
        private string SerializeWithChild(object main, object child) =>
            new ObjectToUrl().SerializeWithChild(main, child, prefix);

        private const string prefix = "prefix:";
        [DataRow((string)null)]
        [DataRow("")]
        [DataRow("icon=hello")]
        [DataRow("icon=hello&value=2")]
        [TestMethod]
        public void FirstOnlyString(string ui)
        {
            AreEqual(ui, SerializeWithChild(ui, null));
        }

        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("prefix:icon=hello", "icon=hello")]
        [DataRow("prefix:icon=hello&prefix:value=2", "icon=hello&value=2")]
        [TestMethod]
        public void ChildOnlyString(string exp, string child)
        {
            AreEqual(exp, SerializeWithChild(null, child));
        }

        [TestMethod]
        public void MainObjectChildString() 
            => AreEqual("id=27&name=daniel&prefix:title=title2", SerializeWithChild(new { id = 27, name = "daniel" }, "title=title2"));

        [TestMethod]
        public void MainStringChildString() 
            => AreEqual("id=27&name=daniel&prefix:title=title2", SerializeWithChild("id=27&name=daniel", "title=title2"));

        [TestMethod]
        public void MainStringChildObject() 
            => AreEqual("id=27&name=daniel&prefix:title=title2", SerializeWithChild("id=27&name=daniel", new { title = "title2"}));

        [TestMethod]
        public void MainObjectChildObject() 
            => AreEqual("id=27&name=daniel&prefix:title=title2", SerializeWithChild(new { id = 27, name = "daniel" }, new { title = "title2"}));
    }
}
