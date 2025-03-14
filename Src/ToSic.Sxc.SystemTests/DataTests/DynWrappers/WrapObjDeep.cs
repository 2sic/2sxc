using Microsoft.CSharp.RuntimeBinder;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjDeep(DynAndTypedTestHelper helper)
{

    [Fact]
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

        dynamic dynAnon = helper.Obj2WrapObj(anon, false, false);
        Equal(anon.Sub, dynAnon.Sub);
    }

    [Fact] public void SubObjectAutoWrappedTrueFalse() 
        => Equal("TF-ok", SubObjectAutoWrapped(true, false, "TF"));

    [Fact] public void SubObjectAutoWrappedTrueTrue() 
        => Equal("TT-ok", SubObjectAutoWrapped(true, true, "TT"));

    [Fact] public void SubObjectAutoWrappedFalseFalse() 
        => Equal("FF-ok", SubObjectAutoWrapped(false, false, "FF"));
    [Fact] public void SubObjectAutoWrappedFalseTrue() 
        => Equal("FT-ok", SubObjectAutoWrapped(false, true, "FT"));

    #region Exeption-tests because wrappings shouldn't work

        

    //[ExpectedException(typeof(RuntimeBinderException))]
    [Fact] public void SubObjectAutoWrappedErrSubFf() =>
        Throws<RuntimeBinderException>(() => Equal("ErrSub-ok", SubObjectAutoWrapped(false, false, "ErrSub")));

    //[ExpectedException(typeof(RuntimeBinderException))]
    [Fact] public void SubObjectAutoWrappedErrSubFt() =>
        Throws<RuntimeBinderException>(() => Equal("ErrSub-ok", SubObjectAutoWrapped(false, true, "ErrSub")));

    //[ExpectedException(typeof(RuntimeBinderException))]
    [Fact] public void SubObjectAutoWrappedErrSubSubFf() =>
        Throws<RuntimeBinderException>(() => Equal("ErrSubSub-ok", SubObjectAutoWrapped(false, false, "ErrSubSub")));

    //[ExpectedException(typeof(RuntimeBinderException))]
    [Fact] public void SubObjectAutoWrappedErrSubSubFt() =>
        Throws<RuntimeBinderException>(() => Equal("ErrSubSub-ok", SubObjectAutoWrapped(false, true, "ErrSubSub")));

    //[ExpectedException(typeof(RuntimeBinderException))]
    [Fact] public void SubObjectAutoWrappedErrSubSomethingFf() =>
        Throws<RuntimeBinderException>(() => Equal("ErrSubSomething-ok", SubObjectAutoWrapped(false, false, "ErrSubSomething")));

    //[ExpectedException(typeof(RuntimeBinderException))]
    [Fact] public void SubObjectAutoWrappedErrSubSomethingFt() =>
        Throws<RuntimeBinderException>(() => Equal("ErrSubSomething-ok", SubObjectAutoWrapped(false, true, "ErrSubSomething")));
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
        dynamic dynWrapAnon = helper.Obj2WrapObj(anon, wrapChildren, wrapRealChildren);

        // These tests should run in all cases
        NotEqual(anon, (object) dynWrapAnon);//, $"wrapper should never be equal {msgPlus}");
        Equal(anon, dynWrapAnon.GetContents());//, $"Wrapped content should be equal {msgPlus}");
        Equal<string[]>(anon.Arr, dynWrapAnon.Arr);//, $"Arrays shouldn't be re-wrapped {msgPlus}");
        Equal(anon.Guid, dynWrapAnon.Guid);//, $"Guids shouldn't be re-wrapped {msgPlus}");

        if (testSet is "TF" or "TT")
        {
            NotEqual<object>(anon.Sub, dynWrapAnon.Sub);//, $"Should not be equal, as the Sub should be re-wrapped {msgPlus}");
            Equal(anon.Sub, dynWrapAnon.Sub.GetContents());//, $"Sub should be = to unwrapped {msgPlus}");
            Equal(typeof(WrapObjectDynamic), dynWrapAnon.Sub.GetType());//, $"type should be {nameof(WrapObjectDynamic)}");
            NotEqual<object>(anon.Sub.Sub, dynWrapAnon.Sub.Sub);//, $"sub-sub shouldn't be equal either, as they are still wrapped {msgPlus}");
            Equal(anon.Sub.Sub, dynWrapAnon.Sub.Sub.GetContents());//, $"sub-sub GetContents {msgPlus}");
            Equal(anon.Sub.Something, dynWrapAnon.sub.something);//, $"simple value, but case-invariant {msgPlus}");
            // added, todo
            Equal(anon.Sub.Something, dynWrapAnon.Sub.something);//, $"simple value, but case-invariant {msgPlus}");
            Equal(anon.Sub.Something, dynWrapAnon.Sub.Something);//, $"simple value, but case-invariant {msgPlus}");
            // </added>
        }

        if (testSet is "TF" or "FF" or "FT")
        {
            Equal(anon.SubNonDynamic, dynWrapAnon.SubNonDynamic);//, $"real object, shouldn't be re-wrapped {msgPlus}");
        }

        if (testSet == "TT")
        {
            NotEqual<object>(anon.SubNonDynamic, dynWrapAnon.SubNonDynamic);//, $"real object, shouldn't be re-wrapped {msgPlus}");
            Equal(anon.SubNonDynamic, dynWrapAnon.SubNonDynamic.GetContents());//, $"real object, shouldn't be re-wrapped {msgPlus}");
        }


        if (testSet is "FT" or "FF")
        {
            Equal(anon.Sub, dynWrapAnon.Sub);//, $"Should not be equal, as the Sub should be re-wrapped {msgPlus}");
            NotEqual<object>(typeof(WrapObjectDynamic), dynWrapAnon.Sub.GetType());//, $"type should be {nameof(WrapObjectDynamic)}");
            Equal(anon.Sub.Sub, dynWrapAnon.Sub.Sub);//, $"sub-sub shouldn't be equal either, as they are still wrapped {msgPlus}");
        }


        // Test wrapping no sub-objects
            
        // All these should throw errors when in FF or FT mode
        if (testSet == "ErrSub")
            Equal(anon.Sub, dynWrapAnon.Sub.GetContents(), $"Sub should be = to unwrapped {msgPlus}");

        if (testSet == "ErrSubSub")
            Equal(anon.Sub.Sub, dynWrapAnon.Sub.Sub.GetContents(), $"sub-sub GetContents {msgPlus}");

        if (testSet == "ErrSubSomething")
            Equal(anon.Sub.Something, dynWrapAnon.sub.something, $"simple value, but case-invariant {msgPlus}");

        return testSet + "-ok";
    }
}