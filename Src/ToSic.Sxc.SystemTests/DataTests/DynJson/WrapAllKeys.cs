using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynJson;


public class WrapAllKeys(DynAndTypedTestHelper helper)
{
    /// <summary>
    /// Base object for many tests - as an anonymous object.
    /// </summary>
    public static readonly object TestDataAnonObject = new
    {
        TrueBoolType = true,
        FalseBoolType = false,
        TrueString = "true",
        FalseString = "false",
        TrueNumber = 1,
        FalseNumber = 0,
        TrueNumberBig = 27,
        TrueNumberNegative = -1,    
    };
        
    /// <summary>
    /// Description of various properties and what they represent (or even if they don't exist)
    /// </summary>
    private static readonly List<PropInfo> BoolKeyTests =
    [
        new("TrueBoolType", true, true, true),
        new("TrueBoolTYPE", true, true, true),
        new("FalseBoolType", true, true, false),
        new("TrueString", true, true, true),
        new("FalseString", true, true, false),
        new("TrueNumber", true, true, true),
        new("FalseNumber", true, true, false),
        new("TrueNumberBig", true, true, true),
        new("TrueNumberNegative", true, true, true),
        new("Something", false, note: "key which doesn't exist"),
        new("Dummy", false, note: "key which doesn't exist"),
        new("Part1", false, note: "key which doesn't exist"),
        new("Dummy.SubDummy", false, note: "key which doesn't exist"),
        new("TrueString.SubDummy", false, note: "key which doesn't exist")
    ];

    public static TheoryData<PropInfo> BoolKeysInfo => [..BoolKeyTests];

    private ITyped BoolTestDataStrict => _boolTestDataStrict ??= helper.Obj2Json2TypedStrict(TestDataAnonObject);
    private static ITyped? _boolTestDataStrict;
    private ITyped BoolTestDataLoose => _boolTestDataLoose ??= helper.Obj2Json2TypedLoose(TestDataAnonObject);
    private static ITyped? _boolTestDataLoose;

        
    [Theory]
    [MemberData(nameof(BoolKeysInfo))]
    public void JsonBoolPropertyKeys_Typed(PropInfo pti) => Equal(pti.Exists, BoolTestDataStrict.TestContainsKey(pti.Name));

    public static TheoryData<PropInfo> BoolKeysExist => [..BoolKeyTests.Where(bk => bk.Exists)];

    [Theory]
    [MemberData(nameof(BoolKeysExist))]
    public void JsonBoolPropertyKeys_Bool(PropInfo pti) =>
        Equal((bool)pti.Value, BoolTestDataStrict.Bool(pti.Name));

    [Theory]
    [MemberData(nameof(BoolKeysInfo))]
    public void IsEmptyBoolData_Json(PropInfo info) =>
        Equal(info.HasData, BoolTestDataStrict.IsNotEmpty(info.Name));//, info.Name);


    #region Test Invalid Keys in different scenarios Typed, Loose, required: false, etc.

    public static TheoryData<PropInfo> BoolInvalidKeys => [..BoolKeyTests.Where(bk => !bk.Exists)];
        
    [Theory]
    [MemberData(nameof(BoolInvalidKeys))]
    public void StrictExceptions_Typed_ShouldError(PropInfo pti) =>
        Throws<ArgumentException>(() => (BoolTestDataStrict.Bool(pti.Name)));

    [Theory]
    [MemberData(nameof(BoolInvalidKeys))]
    public void StrictExceptions_Loose_ShouldRetFalse(PropInfo pti)
        => False(BoolTestDataLoose.Bool(pti.Name));

    [Theory]
    [MemberData(nameof(BoolInvalidKeys))]
    public void StrictExceptions_TypedReqFalse_ShouldRetFalse(PropInfo pti)
        => False(BoolTestDataStrict.Bool(pti.Name, required: false));

    [Theory]
    [MemberData(nameof(BoolInvalidKeys))]
    public void StrictExceptions_TypedReqFalseFallback_ShouldRetFallback(PropInfo pti)
        => True(BoolTestDataStrict.Bool(pti.Name, required: false, fallback: true));

    #endregion

    #region Keys Data Deep

    private static readonly object KeysDataDeepAnon = new
    {
        Key1 = "hello",
        Key2 = "goodbye",
        SubObject = new
        {
            SubTitle = "hello",
            SubSub = new
            {
                SubSubTitle = "hello sub-sub title",
            }
        },
        SubEmpty = new
        {

        }
    };

    /// <summary>
    /// Description of various properties and what they represent (or even if they don't exist)
    /// </summary>
    public static List<PropInfo> KeysDataDeepProps =
    [
        new("Key1", true, true, "hello"),
        new("Key2", true, true, "goodbye"),
        new("Dummy", false, note: "key which doesn't exist"),
        new("SubObject", true, true),
        new("SubObject.SubTitle", true, true, value: "hello"),
        new("SubObject.SubSub", true, true),
        new("SubObject.SubSub.SubSubTitle", true, true, "hello sub-sub title"),
        new("SubObject.SubTitle.Dummy", false),
        new("SubObject.Dummy", false),
        new("SubObject.Dummy.Dummy", false),
        new("SubEmpty", true, hasData: true)
    ];
        
    private ITyped KeysDataObjJsonTyped => helper.Obj2Json2TypedStrict(KeysDataDeepAnon);

    private ITyped KeysDataObjTyped => helper.Obj2Typed(KeysDataDeepAnon);

    #endregion

    #region Tests Data Deep

    [Fact]
    public void KeysCountAny() => True(KeysDataObjJsonTyped.TestKeys().Any());

    [Fact]
    public void KeysCount() => Equal(4, KeysDataObjJsonTyped.TestKeys().Count());

    [Fact]
    public void KeysCountOnlySpecific1() => Equal(1, KeysDataObjJsonTyped.TestKeys(only: ["Key1"]).Count());

    [Fact]
    public void KeysCountOnlySpecific2() => Equal(2, KeysDataObjJsonTyped.TestKeys(only: ["Key1", "Key2"]).Count());

    [Fact]
    public void KeysCountOnlySpecific1of2() => Single(KeysDataObjJsonTyped.TestKeys(only: ["Key1", "KeyNonExisting"]));

    [Fact]
    public void KeysCountOnlySpecific0() => Empty(KeysDataObjJsonTyped.TestKeys(only: ["Nonexisting"]));


    public static TheoryData<PropInfo> KeysDataProps => [..KeysDataDeepProps];
    public static TheoryData<PropInfo> KeysDataPropsExist => [..KeysDataDeepProps.Where(bk => bk.Exists)];

    [Theory]
    [MemberData(nameof(KeysDataPropsExist))]
    public void KeysDataDeepOJT_GetNotNull(PropInfo pti) => NotNull(KeysDataObjJsonTyped.Get(pti.Name));

    [Theory]
    [MemberData(nameof(KeysDataPropsExist))]
    public void KeysDataDeepOT_GetNotNull(PropInfo pti) => NotNull(KeysDataObjTyped.Get(pti.Name));

    [Theory]
    [MemberData(nameof(KeysDataProps))]
    public void KeysDataDeepOJT_ContainsKey(PropInfo pti) => Equal(pti.Exists, KeysDataObjJsonTyped.ContainsKey(pti.Name));

    [Theory]
    [MemberData(nameof(KeysDataProps))]
    public void KeysDataDeepOT_ContainsKey(PropInfo pti) => Equal(pti.Exists, KeysDataObjTyped.ContainsKey(pti.Name));

    [Theory]
    [MemberData(nameof(KeysDataProps))]
    public void KeysDataDeepOJT_IsEmpty(PropInfo pti) => Equal(!pti.HasData, KeysDataObjJsonTyped.IsEmpty(pti.Name));
        
    [Theory]
    [MemberData(nameof(KeysDataProps))]
    public void KeysDataDeepOT_IsEmpty(PropInfo pti) => Equal(!pti.HasData, KeysDataObjTyped.IsEmpty(pti.Name));

    [Theory]
    [MemberData(nameof(KeysDataProps))]
    public void KeysDataDeepOJT_IsNotEmpty(PropInfo pti) => Equal(pti.HasData, KeysDataObjJsonTyped.IsNotEmpty(pti.Name));

    [Theory]
    [MemberData(nameof(KeysDataProps))]
    public void KeysDataDeepOT_IsNotEmpty(PropInfo pti) => Equal(pti.HasData, KeysDataObjTyped.IsNotEmpty(pti.Name));

    #endregion

}