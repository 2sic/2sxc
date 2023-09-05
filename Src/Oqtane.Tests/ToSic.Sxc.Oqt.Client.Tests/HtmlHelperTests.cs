//using System.Xml.Linq;
//using ToSic.Sxc.Oqt.Client.Helpers;

//namespace ToSic.Sxc.Oqt.Client.Tests
//{
//    [TestClass]
//    public class HtmlHelperTests
//    {
//        private static string SetMetaTag(string id, string name, string content) => HtmlHelper.SetMetaTag(id, name, content);
//        private static string GetAttribute(string element, string attribute) => HtmlHelper.GetAttribute(element, attribute);

//        [TestMethod]
//        [DataRow("testId", "testName", "testContent", "<meta id=\"testId\" name=\"testName\" content=\"testContent\" />\n")]
//        [DataRow("testId", "", "testContent", "<meta id=\"testId\" content=\"testContent\" />\n")]
//        [DataRow(null, "testName", "", "<meta name=\"testName\" />\n")]
//        public void SetMetaTag_ValidInput_ReturnsExpectedResult(string id, string name, string content, string expected)
//            => Assert.AreEqual(expected, SetMetaTag(id, name, content));

//        [TestMethod]
//        [DataRow("<meta id=\"testId\" name=\"testName\" content=\"testContent\" />\n", "content", "testContent")]
//        [DataRow("<meta id=\"testId\" name=\"testName\" />\n", "content", null)]
//        public void SetMetaTag_GetAttribute_ReturnsCorrectString(string element, string attribute, string expected)
//            => Assert.AreEqual(expected, GetAttribute(element, attribute));

//        //[TestMethod]
//        //public void GetAttribute_ValidInputs_ReturnsCorrectValue()
//        //{
//        //    // Arrange
//        //    var expected = "test-value";
//        //    var element = $"<test-element test-attribute=\"{expected}\"></test-element>";

//        //    // Act
//        //    var actual = ToSic.Sxc.Oqt.Client.Helpers.HtmlHelper.GetAttribute(element, "test-attribute");

//        //    // Assert
//        //    Assert.AreEqual(expected, actual);
//        //}
//    }
//}