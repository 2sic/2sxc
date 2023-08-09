using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItemAdvanced : DynWrapperTestBase
    {
        private class TestData
        {
            public string ForField { get; set; } = "Hello";

            public string File => "file:72";
        }
        private static readonly TestData Data = new TestData();

        private ITypedItem Item => ItemFromObject(Data);
        private IField Field => Item.Field("ForField");

        [TestMethod] public void FieldExists() => IsNotNull(Field);

        [TestMethod] public void FieldHasName() => AreEqual("ForField", Field.Name);

        [TestMethod] public void FieldHasValue() => AreEqual("Hello", Field.Value);
        [TestMethod] public void FieldHasRaw() => AreEqual("Hello", Field.Raw);

        // File / Folder Tests are more complex, as they need an App context
        // Disabled for now
        //[TestMethod] public void FileExists() => IsNotNull(Item.File("File"));
        //[TestMethod] public void FileIs72() => AreEqual(72, Item.File("File").Id);
    }
}
