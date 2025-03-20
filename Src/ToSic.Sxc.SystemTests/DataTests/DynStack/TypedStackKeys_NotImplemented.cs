using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynStack;


public class TypedStackKeys_NotImplemented(DynAndTypedTestHelper helper)
{
    public static TheoryData<PropInfo> StackProps => [..TypedStackTestData.StackOrder12PropInfo];//.ToTestEnum();

    [Theory]
    [MemberData(nameof(StackProps))]
    //[ExpectedException(typeof(NotImplementedException))]
    public void Keys_AnonObjects(PropInfo pti) =>
        Throws<NotImplementedException>(() => Equal(pti.Exists, StackForKeysFromAnon.ContainsKey(pti.Name)));


#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    private ITypedStack StackForKeysFromAnon => field ??= TypedStackTestData.GetStackForKeysUsingAnon(helper);
}