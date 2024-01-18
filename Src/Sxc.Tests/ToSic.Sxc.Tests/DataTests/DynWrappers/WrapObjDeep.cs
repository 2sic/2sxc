using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Dynamic;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjDeep: DynAndTypedTestsBase
    {

        [TestMethod]
        public void SubObjectNotWrapped()
        {
            var anon = new
            {
                Simple = "simple",
                Sub = new
                {
                    SubSub = "Test"
                }
            };

            var dynAnon = Obj2WrapObj(anon, false, false) as dynamic;
            AreEqual(anon.Sub, dynAnon.Sub);
        }

        [TestMethod] public void SubObjectAutoWrappedTrueFalse() 
            => AreEqual("TF-ok", SubObjectAutoWrapped(true, false, "TF"));

        [TestMethod] public void SubObjectAutoWrappedTrueTrue() 
            => AreEqual("TT-ok", SubObjectAutoWrapped(true, true, "TT"));

        [TestMethod] public void SubObjectAutoWrappedFalseFalse() 
            => AreEqual("FF-ok", SubObjectAutoWrapped(false, false, "FF"));
        [TestMethod] public void SubObjectAutoWrappedFalseTrue() 
            => AreEqual("FT-ok", SubObjectAutoWrapped(false, true, "FT"));

        #region Exeption-tests because wrappings shouldn't work

        

        [ExpectedException(typeof(RuntimeBinderException))]
        [TestMethod] public void SubObjectAutoWrappedErrSubFf() 
            => AreEqual("ErrSub-ok", SubObjectAutoWrapped(false, false, "ErrSub"));
        [ExpectedException(typeof(RuntimeBinderException))]
        [TestMethod] public void SubObjectAutoWrappedErrSubFt() 
            => AreEqual("ErrSub-ok", SubObjectAutoWrapped(false, true, "ErrSub"));

        [ExpectedException(typeof(RuntimeBinderException))]
        [TestMethod] public void SubObjectAutoWrappedErrSubSubFf() 
            => AreEqual("ErrSubSub-ok", SubObjectAutoWrapped(false, false, "ErrSubSub"));
        [ExpectedException(typeof(RuntimeBinderException))]
        [TestMethod] public void SubObjectAutoWrappedErrSubSubFt() 
            => AreEqual("ErrSubSub-ok", SubObjectAutoWrapped(false, true, "ErrSubSub"));

        [ExpectedException(typeof(RuntimeBinderException))]
        [TestMethod] public void SubObjectAutoWrappedErrSubSomethingFf() 
            => AreEqual("ErrSubSomething-ok", SubObjectAutoWrapped(false, false, "ErrSubSomething"));
        [ExpectedException(typeof(RuntimeBinderException))]
        [TestMethod] public void SubObjectAutoWrappedErrSubSomethingFt() 
            => AreEqual("ErrSubSomething-ok", SubObjectAutoWrapped(false, true, "ErrSubSomething"));
        #endregion

        private string SubObjectAutoWrapped(bool wrapChildren, bool wrapRealChildren, string testSet)
        {
            var anon = new
            {
                Simple = "simple",
                Sub = new
                {
                    Something = "Test",
                    Sub = new {
                        Sub = new { }
                    }
                },
                Guid = new Guid(),
                SubNonDynamic = new List<string>(),
                Arr = Array.Empty<string>(),
            };

            // Test wrapping anonymous sub-objects only
            var msgPlus = $" - DynRead(..., {wrapChildren}, {wrapRealChildren})";
            var dynWrapAnon = Obj2WrapObj(anon, wrapChildren, wrapRealChildren) as dynamic;

            // These tests should run in all cases
            AreNotEqual(anon, dynWrapAnon, $"wrapper should never be equal {msgPlus}");
            AreEqual(anon, dynWrapAnon.GetContents(), $"Wrapped content should be equal {msgPlus}");
            AreEqual(anon.Arr, dynWrapAnon.Arr, $"Arrays shouldn't be re-wrapped {msgPlus}");
            AreEqual(anon.Guid, dynWrapAnon.Guid, $"Guids shouldn't be re-wrapped {msgPlus}");

            if (testSet == "TF" || testSet == "TT")
            {
                AreNotEqual(anon.Sub, dynWrapAnon.Sub, $"Should not be equal, as the Sub should be re-wrapped {msgPlus}");
                AreEqual(anon.Sub, dynWrapAnon.Sub.GetContents(), $"Sub should be = to unwrapped {msgPlus}");
                AreEqual(typeof(WrapObjectDynamic), dynWrapAnon.Sub.GetType(), $"type should be {nameof(WrapObjectDynamic)}");
                AreNotEqual(anon.Sub.Sub, dynWrapAnon.Sub.Sub, $"sub-sub shouldn't be equal either, as they are still wrapped {msgPlus}");
                AreEqual(anon.Sub.Sub, dynWrapAnon.Sub.Sub.GetContents(), $"sub-sub GetContents {msgPlus}");
                AreEqual(anon.Sub.Something, dynWrapAnon.sub.something, $"simple value, but case-invariant {msgPlus}");
                // added, todo
                AreEqual(anon.Sub.Something, dynWrapAnon.Sub.something, $"simple value, but case-invariant {msgPlus}");
                AreEqual(anon.Sub.Something, dynWrapAnon.Sub.Something, $"simple value, but case-invariant {msgPlus}");
                // </added>
            }

            if (testSet == "TF" || testSet == "FF" || testSet == "FT")
            {
                AreEqual(anon.SubNonDynamic, dynWrapAnon.SubNonDynamic, $"real object, shouldn't be re-wrapped {msgPlus}");
            }

            if (testSet == "TT")
            {
                AreNotEqual(anon.SubNonDynamic, dynWrapAnon.SubNonDynamic, $"real object, shouldn't be re-wrapped {msgPlus}");
                AreEqual(anon.SubNonDynamic, dynWrapAnon.SubNonDynamic.GetContents(), $"real object, shouldn't be re-wrapped {msgPlus}");
            }


            if (testSet == "FT" || testSet == "FF")
            {
                AreEqual(anon.Sub, dynWrapAnon.Sub, $"Should not be equal, as the Sub should be re-wrapped {msgPlus}");
                AreNotEqual(typeof(WrapObjectDynamic), dynWrapAnon.Sub.GetType(), $"type should be {nameof(WrapObjectDynamic)}");
                AreEqual(anon.Sub.Sub, dynWrapAnon.Sub.Sub, $"sub-sub shouldn't be equal either, as they are still wrapped {msgPlus}");
            }


            // Test wrapping no sub-objects
            
            // All these should throw errors when in FF or FT mode
            if (testSet == "ErrSub")
                AreEqual(anon.Sub, dynWrapAnon.Sub.GetContents(), $"Sub should be = to unwrapped {msgPlus}");

            if (testSet == "ErrSubSub")
                AreEqual(anon.Sub.Sub, dynWrapAnon.Sub.Sub.GetContents(), $"sub-sub GetContents {msgPlus}");

            if (testSet == "ErrSubSomething")
                AreEqual(anon.Sub.Something, dynWrapAnon.sub.something, $"simple value, but case-invariant {msgPlus}");

            return testSet + "-ok";
        }
    }
}
