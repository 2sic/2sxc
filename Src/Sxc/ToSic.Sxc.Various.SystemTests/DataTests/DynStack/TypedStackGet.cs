using ToSic.Eav.Data.PropertyStack.Sys;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.DynamicJacket;
using ToSic.Sxc.Data.Sys.Wrappers;
using static ToSic.Sxc.DataTests.DynStack.TypedStackTestData;

namespace ToSic.Sxc.DataTests.DynStack;


public class TypedStackGet(DynAndTypedTestHelper helper)
{
    private ITypedStack StackFromAnon => GetStackForKeysUsingAnon(helper);

#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    private ITypedStack StackFromJson => field ??= GetStackForKeysUsingJson(helper);


    public static TheoryData<PropInfo> StackProps => [..StackOrder12PropInfo];
    public static TheoryData<PropInfo> StackPropsWithValue => [..StackOrder12PropInfo.Where(sp => sp.HasData && sp.Value?.ToString() != ValueNotTestable)];


    [Theory]
    [MemberData(nameof(StackProps))]
    public void IsNotEmpty_BasedOnAnon(PropInfo pti)
        => Equal(pti.HasData, StackFromAnon.IsNotEmpty(pti.Name));//, pti.ToString());

    [Theory]
    [MemberData(nameof(StackProps))]
    public void ExistsNotNull_FromAnon_ReqFalse(PropInfo pti) 
        => Equal(pti.Exists, StackFromAnon.Get(pti.Name, required: false) != null);

    [Theory]
    [MemberData(nameof(StackPropsWithValue))]
    public void ExistsNotNull_FromJson_ReqFalse(PropInfo pti)
        => Equal(pti.Exists, StackFromJson.Get(pti.Name, required: false) != null);

    [Theory]
    [MemberData(nameof(StackProps))]
    public void IsNotEmpty_FromAnon_ReqFalse(PropInfo pti) 
        => Equal(pti.Exists, StackFromAnon.IsNotEmpty(pti.Name));

    [Theory]
    [MemberData(nameof(StackPropsWithValue))]
    public void IsNotEmpty_FromJson_ReqFalse(PropInfo pti)
        => Equal(pti.Exists, StackFromJson.IsNotEmpty(pti.Name));

    [Theory]
    [MemberData(nameof(StackPropsWithValue))]
    public void Get_FromJson_ReqFalse(PropInfo pti)
        => Equal(pti.Value, StackFromJson.Get(pti.Name, required: false));


    [Theory]
    [MemberData(nameof(StackPropsWithValue))]
    public void Get_FromAnon_ReqFalse(PropInfo pti) 
        => Equal(pti.Value, StackFromAnon.Get(pti.Name, required: false));


    #region Verify that what's inside the object matches expectation

    [Fact]
    public void Verify_StackFromAnonWrapsObjectTyped()
    {
        var inspect = (IWrapper<IPropertyStack>)StackFromAnon;
        IsType<PreWrapObject>(inspect.GetContents().Sources.FirstOrDefault().Value);
    }


    [Fact]
    public void Verify_StackFromJsonWrapsJacket()
    {
        var inspect = (IWrapper<IPropertyStack>)StackFromJson;
        IsType<PreWrapJsonObject>(inspect.GetContents().Sources.FirstOrDefault().Value);
    }


    #endregion

}