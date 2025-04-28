using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjTypedItemAdvanced(DynAndTypedTestHelper helper)
{
    private class TestData
    {
        public string ForField { get; set; } = "Hello";

        public string File => "file:72";

    }
    private static readonly TestData Data = new();

    private ITypedItem Item => helper.Obj2Item(Data);
    private IField Field => Item.Field("ForField");

    [Fact] public void FieldExists() => NotNull(Field);

    [Fact] public void FieldHasName() => Equal("ForField", Field.Name);

    [Fact] public void FieldHasUrl() => Equal("Hello", Field.Url);
    [Fact] public void FieldHasRaw() => Equal("Hello", Field.Raw);
        
    // File / Folder Tests are more complex, as they need an App context
    // Disabled for now
    //[Fact] public void FileExists() => NotNull(Item.File("File"));
    //[Fact] public void FileIs72() => Equal(72, Item.File("File").Id);
}