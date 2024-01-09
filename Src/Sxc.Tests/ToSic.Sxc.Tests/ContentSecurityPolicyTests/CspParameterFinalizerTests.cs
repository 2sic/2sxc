using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests
{
    [TestClass]
    public class CspParameterFinalizerTests
    {
        private readonly CspParameterFinalizer _finalizer = new();//null);

        [TestMethod]
        public void NothingHasNoDefault()
        {
            var cspp = new CspParameters();
            cspp = _finalizer.MergedWithAll(cspp);
            Assert.AreEqual("", cspp.ToString());
        }

        [TestMethod]
        public void AllEmptyDoesNotInitializeDefault()
        {
            var cspp = new CspParameters
            {
                { CspConstants.AllSrcName, "" }
            };
            cspp = _finalizer.MergedWithAll(cspp);
            Assert.AreEqual("", cspp.ToString());
        }

        [TestMethod]
        public void AllInitializesDefault()
        {
            var cspp = new CspParameters
            {
                { CspConstants.AllSrcName, "'self'" }
            };
            cspp = _finalizer.MergedWithAll(cspp);
            Assert.AreEqual("default-src 'self';", cspp.ToString());
        }

        [TestMethod]
        public void AllExtendsDefault()
        {
            var cspp = new CspParameters
            {
                { CspConstants.DefaultSrcName, "'none'"},
                { CspConstants.AllSrcName, "'self'" }
            };
            cspp = _finalizer.MergedWithAll(cspp);
            Assert.AreEqual("default-src 'none' 'self';", cspp.ToString());
        }

        [TestMethod]
        public void FinalizeWithoutDuplicates()
        {
            var cspp = new CspParameters
            {
                { CspConstants.DefaultSrcName, "'none'"},
                { CspConstants.DefaultSrcName, "'self'"},
                { CspConstants.AllSrcName, "'self'" }
            };
            cspp = _finalizer.Finalize(cspp);
            Assert.AreEqual("default-src 'none' 'self';", cspp.ToString());
        }

        [TestMethod]
        public void DeduplicateEmpty()
        {
            var cspp = new CspParameters();
            cspp = _finalizer.DeduplicateValues(cspp);
            Assert.AreEqual("", cspp.ToString());
        }

        [TestMethod]
        public void DeduplicateNoDuplicates()
        {
            var cspp = new CspParameters
            {
                { CspConstants.AllSrcName, "test" },
                { CspConstants.AllSrcName, "test2" },
            };
            cspp = _finalizer.DeduplicateValues(cspp);
            Assert.AreEqual("all-src test test2;", cspp.ToString());
        }

        [TestMethod]
        public void DeduplicateDuplicates()
        {
            var cspp = new CspParameters
            {
                { CspConstants.AllSrcName, "test" },
                { CspConstants.AllSrcName, "test" },
            };
            cspp = _finalizer.DeduplicateValues(cspp);
            Assert.AreEqual("all-src test;", cspp.ToString());
        }


    }
}
