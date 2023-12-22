using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItemChildren : DynAndTypedTestsBase
    {

        private class TestData
        {
            public TestPerson Author { get; } = new()
            {
                Id = 101,
                Title = "some author",
                FirstName = "Some"
            };

            public TestPerson[] Readers { get; } = {
                new()
                {
                    Id = 201
                },
                new()
                {
                    Id = 202
                },
                new()
                {
                    Id = 301,
                    Type = "Company"
                },
            };
        }
        private static readonly TestData Data = new();
        private ITypedItem Item => Obj2Item(Data);
        private ITypedItem Author() => Item.Child("author");
        private ITypedItem Reader() => Item.Child("readers");
        private IEnumerable<ITypedItem> Readers() => Item.Children("readers");

        [TestMethod] public void ChildSingleExists() => IsNotNull(Author());
        [TestMethod] public void ChildDummyNotExists() => IsNull(Item.Child("dummy"));
        [TestMethod] public void ChildSingleId() => AreEqual(Data.Author.Id, Author().Id);
        [TestMethod] public void ChildSingleTitle() => AreEqual(Data.Author.Title, Author().Title);
        [TestMethod] public void ChildSingleFirstName() => AreEqual(Data.Author.FirstName, Author().String("firstname"));

        [TestMethod] public void ReaderSingleExists() => IsNotNull(Reader());
        [TestMethod] public void ReaderSingleId() => AreEqual(Data.Readers[0].Id, Reader().Id);

        [TestMethod] public void ReadersListExists() => IsNotNull(Readers());

        [TestMethod] public void ReadersListCount() => AreEqual(Data.Readers.Length, Readers().Count());
        [TestMethod] public void ReadersPersonCount() => AreEqual(Data.Readers.Length - 1, Item.Children("readers", type: "Person").Count());
        [TestMethod] public void ReadersCompanyCount() => AreEqual(1, Item.Children("readers", type: "Company").Count());
        [TestMethod] public void FakeListNotExists() => IsNotNull(Item.Children("dummy"));
        [TestMethod] public void ReadersList1Exists() => IsNotNull(Readers().First());
        [TestMethod] public void ReadersList1Id() => AreEqual(Data.Readers[0].Id, Readers().First().Id);
        [TestMethod] public void ReadersList2Exists() => IsNotNull(Readers().Skip(1).First());

    }
}
