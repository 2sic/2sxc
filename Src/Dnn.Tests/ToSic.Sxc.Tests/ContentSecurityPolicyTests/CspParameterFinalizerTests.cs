using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Tests.ContentSecurityPolicyTests
{
    [TestClass]
    public class CspParameterFinalizerTests
    {
        private readonly CspParameterFinalizer _finalizer = new CspParameterFinalizer();

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
                { CspService.AllSrcName, "" }
            };
            cspp = _finalizer.MergedWithAll(cspp);
            Assert.AreEqual("", cspp.ToString());
        }

        [TestMethod]
        public void AllInitializesDefault()
        {
            var cspp = new CspParameters
            {
                { CspService.AllSrcName, "'self'" }
            };
            cspp = _finalizer.MergedWithAll(cspp);
            Assert.AreEqual("default-src 'self';", cspp.ToString());
        }

        [TestMethod]
        public void AllExtendsDefault()
        {
            var cspp = new CspParameters
            {
                { CspService.DefaultSrcName, "'none'"},
                { CspService.AllSrcName, "'self'" }
            };
            cspp = _finalizer.MergedWithAll(cspp);
            Assert.AreEqual("default-src 'none' 'self';", cspp.ToString());
        }

        [TestMethod]
        public void FinalizeWithoutDuplicates()
        {
            var cspp = new CspParameters
            {
                { CspService.DefaultSrcName, "'none'"},
                { CspService.DefaultSrcName, "'self'"},
                { CspService.AllSrcName, "'self'" }
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
                { CspService.AllSrcName, "test" },
                { CspService.AllSrcName, "test2" },
            };
            cspp = _finalizer.DeduplicateValues(cspp);
            Assert.AreEqual("all-src test test2;", cspp.ToString());
        }

        [TestMethod]
        public void DeduplicateDuplicates()
        {
            var cspp = new CspParameters
            {
                { CspService.AllSrcName, "test" },
                { CspService.AllSrcName, "test" },
            };
            cspp = _finalizer.DeduplicateValues(cspp);
            Assert.AreEqual("all-src test;", cspp.ToString());
        }


    }
}
