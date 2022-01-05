using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class LinkImageParameters: LinkImageTestBase
    {
        [TestMethod]
        public void BasicParameters()
        {
            AreEqual("test.jpg?name=daniel", GetLinkHelper().Image("test.jpg", parameters: "name=daniel"));
        }

        [TestMethod]
        public void KeyOnly()
        {
            AreEqual("test.jpg?active", GetLinkHelper().Image("test.jpg", parameters: "active"));
        }

        [TestMethod]
        public void AddKeyToExisting()
        {
            AreEqual("test.jpg?wx=200&active", GetLinkHelper().Image("test.jpg?wx=200", parameters: "active"));
        }

        [TestMethod]
        public void AddPairToExisting()
        {
            AreEqual("test.jpg?wx=200&active=true", GetLinkHelper().Image("test.jpg?wx=200", parameters: "active=true"));
        }


        [TestMethod]
        public void AddPairToWh()
        {
            AreEqual("test.jpg?w=200&h=200&active=true", GetLinkHelper().Image("test.jpg", width: 200, height: 200, parameters: "active=true"));
        }

        [TestMethod]
        public void AddPairToWhAndExisting()
        {
            AreEqual("test.jpg?wx=700&w=200&h=200&active=true", GetLinkHelper().Image("test.jpg?wx=700", width: 200, height: 200, parameters: "active=true"));
        }
    }
}
