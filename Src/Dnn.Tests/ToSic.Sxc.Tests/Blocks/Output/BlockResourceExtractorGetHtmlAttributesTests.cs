using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Tests.Blocks.Output
{
    [TestClass()]
    public class BlockResourceExtractorGetHtmlAttributesTests
    {
        private IDictionary<string, string> GetHtmlAttributes(string htmlTag) => BlockResourceExtractor.GetHtmlAttributes(htmlTag);


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
            var rez1 = GetHtmlAttributes("<tag key/>");
            var rez2 = GetHtmlAttributes("<tag     key/>");
            var rez3 = GetHtmlAttributes("<tag    key    />");
            var rez4 = GetHtmlAttributes("<tag \n \t   key    />");
            var expected = new Dictionary<string, string> { { "key", "" } };
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
            CollectionAssert.AreEquivalent(rez2.ToList(), expected.ToList());
            CollectionAssert.AreEquivalent(rez3.ToList(), expected.ToList());
            CollectionAssert.AreEquivalent(rez4.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ManyAttributesWithoutValueTests()
        {
            var rez = GetHtmlAttributes("<tag key1 key2 \n key3    Key4  \t  KEY5/>");
            var expected = new Dictionary<string, string>
            {
                { "key1", "" },
                { "key2", "" }, 
                { "key3", "" },
                { "Key4", "" },
                { "KEY5", "" }
            };
            CollectionAssert.AreEquivalent(rez.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ManyDuplicateAttributesWithoutValueTests()
        {
            var rez = GetHtmlAttributes("<tag key1  key1 \n key1    Key1   KEY1/>");
            var expected = new Dictionary<string, string>
            {
                { "key1", "" },
                { "Key1", "" },
                { "KEY1", "" }
            };
            CollectionAssert.AreEquivalent(rez.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void OneAttributeWithValueTests()
        {
            var rez1 = GetHtmlAttributes("<tag key=\"value\"/>");
            var rez2 = GetHtmlAttributes("<tag key='value'   />");
            //var rez3 = GetHtmlAttributes("<tag key=value/>");
            var expected = new Dictionary<string, string> {{"key", "value"}};
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
            CollectionAssert.AreEquivalent(rez2.ToList(), expected.ToList());
            //CollectionAssert.AreEquivalent(rez3.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ManyAttributesTests()
        {
            var rez1 = GetHtmlAttributes("<tag key1 key2=\"value\"   \n  key3=\"value\"     />");
            var expected = new Dictionary<string, string>
            {
                { "key1", "" } , 
                { "key2", "value" },
                { "key3", "value" }
            };
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ScriptOnlyAdditionalAttributesTests()
        {
            var rez1 = GetHtmlAttributes("<script type='module' async crossorigin='anonymous' defer integrity='filehash' nomodule='false' referrerpolicy='strict-origin-when-cross-origin' ></script>");
            var expected = new Dictionary<string, string>
            {
                { "type", "module" } ,
                { "async", "" },
                { "crossorigin", "anonymous" },
                { "defer", "" },
                { "integrity", "filehash" },
                { "nomodule", "false" },
                { "referrerpolicy", "strict-origin-when-cross-origin" },
            };
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ScriptOnlyAdditionalAttributesNoValueTests()
        {
            var rez1 = GetHtmlAttributes("<script async defer></script>");
            var expected = new Dictionary<string, string>
            {
                { "async", "" },
                { "defer", "" }
            };
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ScriptOnlyAdditionalAttributesWithValueTests()
        {
            var rez1 = GetHtmlAttributes("<script async='async' defer='defer'></script>");
            var expected = new Dictionary<string, string>
            {
                { "async", "async" },
                { "defer", "defer" }
            };
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ScriptOnlyBlacklistAttributesTests()
        {
            var rez1 = GetHtmlAttributes("<script id='id' src='src' data-enableoptimizations='true' ></script>");
            var expected = new Dictionary<string, string> { };
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
        }

        [TestMethod()]
        public void ScriptMixAttributesTests()
        {
            var rez1 = GetHtmlAttributes("<script id='id' async src='src' defer data-enableoptimizations='true' ></script>");
            var expected = new Dictionary<string, string>
            {
                { "async", "" },
                { "defer", "" }
            };
            CollectionAssert.AreEquivalent(rez1.ToList(), expected.ToList());
        }
    }
}