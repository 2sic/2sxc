using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Tests.Blocks.Output
{
    [TestClass()]
    public class BlockResourceExtractorGetHtmlAttributesTests
    {
 
        private Dictionary<string, string> GetHtmlAttributes(string htmlTag) => BlockResourceExtractor.GetHtmlAttributes(htmlTag) as Dictionary<string, string>;


        [TestMethod()]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("<tag/>")]
        [DataRow("    <tag \n  \n     />    ")]
        //[DataRow("    <   tag      />    ")]
        public void TagWithoutAttributesTest(string htmlTag)
        {
            Assert.IsNull(GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<tag key/>")]
        [DataRow("<tag     key/>")]
        [DataRow("<tag    key    />")]
        [DataRow("<tag \n \t   key    />")]
        public void OneAttributeWithoutValueTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "key", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<tag key1 key2 \n key3    Key4  \t  KEY5/>")]
        public void ManyAttributesWithoutValueTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "key1", "" },
                { "key2", "" }, 
                { "key3", "" },
                { "key4", "" },
                { "key5", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<tag key1  key1 \n key1    Key1   KEY1/>")]
        public void ManyDuplicateAttributesWithoutValueTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "key1", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<tag key=\"value\"/>")]
        [DataRow("<tag key='value'   />")]
        //[DataRow("<tag key=value/>")]
        public void OneAttributeWithValueTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                {"key", "value"}
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<tag key1 key2=\"value\"   \n  key3=\"value\"     />")]
        public void ManyAttributesTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "key1", "" } , 
                { "key2", "value" },
                { "key3", "value" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script type='module' async crossorigin='anonymous' defer integrity='filehash' nomodule='false' referrerpolicy='strict-origin-when-cross-origin'></script>")]
        public void ScriptOnlyAdditionalAttributesTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "type", "module" } ,
                { "async", "" },
                { "crossorigin", "anonymous" },
                { "defer", "" },
                { "integrity", "filehash" },
                { "nomodule", "false" },
                { "referrerpolicy", "strict-origin-when-cross-origin" },
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script async defer></script>")]
        public void ScriptOnlyAdditionalAttributesNoValueTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "async", "" },
                { "defer", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script async='async' defer='defer'></script>")]
        public void ScriptOnlyAdditionalAttributesWithValueTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "async", "async" },
                { "defer", "defer" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script id='id' src='src' data-enableoptimizations='true'></script>")]
        public void ScriptOnlyBlacklistAttributesTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>() { };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script id='id' async src='src' defer data-enableoptimizations='true'></script>")]
        public void ScriptMixAttributesTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "async", "" },
                { "defer", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script src='src' onload=\"loaded=1\" onerror=\"return false;\"></script>")]
        public void ScriptEventsTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "onload", "loaded=1" },
                { "onerror", "return false;" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }


        [TestMethod()]
        [DataRow("<script src='src' onerror=\"alert('error!')\"></script>")]
        [Ignore("ATM not ready, javascript value extraction is not working")]
        public void ScriptEventsThatFailsTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "onerror", "alert('error!')" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script src='src' data-edit-context=\"{\"Environment\":{\"WebsiteId\":74,\"WebsiteUrl\":\"//2sxc-dnn742.dnndev.me/script-extractor/\",\"PageId\":2406,\"PageUrl\":\"http://2sxc-dnn742.dnndev.me/script-extractor/t2\",\"parameters\":[],\"InstanceId\":4140,\"SxcVersion\":\"13.3.0.1646933502\",\"SxcRootUrl\":\"/\",\"IsEditable\":true},\"User\":{\"CanDevelop\":true,\"CanAdmin\":true},\"Language\":{\"Current\":\"en-us\",\"Primary\":\"en-us\",\"All\":[]},\"contentBlockReference\":{\"publishingMode\":\"DraftOptional\",\"id\":4140,\"parentIndex\":0,\"partOfPage\":true},\"contentBlock\":{\"IsCreated\":false,\"IsList\":false,\"TemplateId\":80573,\"QueryId\":null,\"ContentTypeName\":\"\",\"AppUrl\":\"/Portals/script-extractor/2sxc/ScriptExtractorAndBoundlingTest\",\"AppSettingsId\":null,\"AppResourcesId\":null,\"IsContent\":false,\"HasContent\":false,\"SupportsAjax\":false,\"TemplatePath\":\"/_v2.cshtml\",\"TemplateIsShared\":false,\"ZoneId\":77,\"AppId\":853,\"Guid\":\"00000000-0000-0000-0000-000000000000\",\"Id\":0},\"error\":{\"type\":null},\"Ui\":{\"AutoToolbar\":true}}\"></script>")]
        [Ignore("ATM not ready, JSON value extraction is not working")]
        public void ScriptAttributeWithJsonThatFailsTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "data-edit-context", "{\"Environment\":{\"WebsiteId\":74,\"WebsiteUrl\":\"//2sxc-dnn742.dnndev.me/script-extractor/\",\"PageId\":2406,\"PageUrl\":\"http://2sxc-dnn742.dnndev.me/script-extractor/t2\",\"parameters\":[],\"InstanceId\":4140,\"SxcVersion\":\"13.3.0.1646933502\",\"SxcRootUrl\":\"/\",\"IsEditable\":true},\"User\":{\"CanDevelop\":true,\"CanAdmin\":true},\"Language\":{\"Current\":\"en-us\",\"Primary\":\"en-us\",\"All\":[]},\"contentBlockReference\":{\"publishingMode\":\"DraftOptional\",\"id\":4140,\"parentIndex\":0,\"partOfPage\":true},\"contentBlock\":{\"IsCreated\":false,\"IsList\":false,\"TemplateId\":80573,\"QueryId\":null,\"ContentTypeName\":\"\",\"AppUrl\":\"/Portals/script-extractor/2sxc/ScriptExtractorAndBoundlingTest\",\"AppSettingsId\":null,\"AppResourcesId\":null,\"IsContent\":false,\"HasContent\":false,\"SupportsAjax\":false,\"TemplatePath\":\"/_v2.cshtml\",\"TemplateIsShared\":false,\"ZoneId\":77,\"AppId\":853,\"Guid\":\"00000000-0000-0000-0000-000000000000\",\"Id\":0},\"error\":{\"type\":null},\"Ui\":{\"AutoToolbar\":true}}" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }

        [TestMethod()]
        [DataRow("<script src='src' multiline=\"line1\nline2\nline3\"></script>")]
        [Ignore("ATM not ready, multiline value extraction is not working")]
        public void ScriptMultilineAttributeThatFailsTests(string htmlTag)
        {
            var expected = new Dictionary<string, string>()
            {
                { "multiline", "line1\nline2\nline3" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes(htmlTag));
        }
    }
}