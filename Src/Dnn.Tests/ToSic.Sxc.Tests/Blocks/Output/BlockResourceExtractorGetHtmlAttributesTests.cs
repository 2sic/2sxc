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
        public void TagWithoutAttributesTest()
        {
            Assert.IsNull(GetHtmlAttributes(null));
            Assert.IsNull(GetHtmlAttributes(""));
            Assert.IsNull(GetHtmlAttributes("   "));
            Assert.IsNull(GetHtmlAttributes("<tag/>"));
            Assert.IsNull(GetHtmlAttributes("    <tag \n  \n     />    "));
            //Assert.IsNull(GetHtmlAttributes("    <   tag      />    "));
        }

        [TestMethod()]
        public void OneAttributeWithoutValueTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "key", "" }
            };

            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag key/>"));
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag     key/>"));
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag    key    />"));
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag \n \t   key    />"));
        }

        [TestMethod()]
        public void ManyAttributesWithoutValueTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "key1", "" },
                { "key2", "" }, 
                { "key3", "" },
                { "key4", "" },
                { "key5", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag key1 key2 \n key3    Key4  \t  KEY5/>"));
        }

        [TestMethod()]
        public void ManyDuplicateAttributesWithoutValueTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "key1", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag key1  key1 \n key1    Key1   KEY1/>"));
        }

        [TestMethod()]
        public void OneAttributeWithValueTests()
        {
            var expected = new Dictionary<string, string>()
            {
                {"key", "value"}
            };
            
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag key=\"value\"/>"));
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag key='value'   />"));
            //CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag key=value/>"));
        }

        [TestMethod()]
        public void ManyAttributesTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "key1", "" } , 
                { "key2", "value" },
                { "key3", "value" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<tag key1 key2=\"value\"   \n  key3=\"value\"     />"));
        }

        [TestMethod()]
        public void ScriptOnlyAdditionalAttributesTests()
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
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<script type='module' async crossorigin='anonymous' defer integrity='filehash' nomodule='false' referrerpolicy='strict-origin-when-cross-origin'></script>"));
        }

        [TestMethod()]
        public void ScriptOnlyAdditionalAttributesNoValueTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "async", "" },
                { "defer", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<script async defer></script>"));
        }

        [TestMethod()]
        public void ScriptOnlyAdditionalAttributesWithValueTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "async", "async" },
                { "defer", "defer" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<script async='async' defer='defer'></script>"));
        }

        [TestMethod()]
        public void ScriptOnlyBlacklistAttributesTests()
        {
            var expected = new Dictionary<string, string>() { };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<script id='id' src='src' data-enableoptimizations='true'></script>"));
        }

        [TestMethod()]
        public void ScriptMixAttributesTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "async", "" },
                { "defer", "" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<script id='id' async src='src' defer data-enableoptimizations='true'></script>"));
        }

        [TestMethod()]
        public void ScriptEventsTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "onload", "loaded=1" },
                { "onerror", "return false;" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<script src='src' onload=\"loaded=1\" onerror=\"return false;\"></script>"));
        }


        [TestMethod()]
        [Ignore("ATM not ready, complicated value extraction is not working")]
        public void ScriptEventsThatFailsTests()
        {
            var expected = new Dictionary<string, string>()
            {
                { "onerror", "alert('error!')" }
            };
            CollectionAssert.AreEquivalent(expected, GetHtmlAttributes("<script src='src' onerror=\"alert('error!')\"></script>"));
        }
    }
}