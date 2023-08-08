using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItemChildren : DynWrapperTestBase
    {

        private class TestData
        {
            public TestPerson Author { get; } = new TestPerson
            {
                Id = 101,
                Title = "some author",
                FirstName = "Some"
            };

            public TestPerson[] Readers { get; } = {
                new TestPerson
                {
                    Id = 201
                },
                new TestPerson
                {
                    Id = 202
                }
            };
        }
        private static readonly TestData Data = new TestData();

        private ITypedItem Author() => ItemFromObject(Data).Child("author");
        private ITypedItem Reader() => ItemFromObject(Data).Child("readers");
        private IEnumerable<ITypedItem> Readers() => ItemFromObject(Data).Children("readers");

        [TestMethod] public void ChildSingleExists() => IsNotNull(Author());
        [TestMethod] public void ChildDummyNotExists() => IsNull(ItemFromObject(Data).Child("dummy"));
        [TestMethod] public void ChildSingleId() => AreEqual(Data.Author.Id, Author().Id);
        [TestMethod] public void ChildSingleTitle() => AreEqual(Data.Author.Title, Author().Title);
        [TestMethod] public void ChildSingleFirstName() => AreEqual(Data.Author.FirstName, Author().String("firstname"));

        [TestMethod] public void ReaderSingleExists() => IsNotNull(Reader());
        [TestMethod] public void ReaderSingleId() => AreEqual(Data.Readers[0].Id, Reader().Id);

        [TestMethod] public void ReadersListExists() => IsNotNull(Readers());

        [TestMethod] public void ReadersListCount() => AreEqual(Data.Readers.Length, Readers().Count());
        [TestMethod] public void FakeListNotExists() => IsNotNull(ItemFromObject(Data).Children("dummy"));
        [TestMethod] public void ReadersList1Exists() => IsNotNull(Readers().First());
        [TestMethod] public void ReadersList1Id() => AreEqual(Data.Readers[0].Id, Readers().First().Id);
        [TestMethod] public void ReadersList2Exists() => IsNotNull(Readers().Skip(1).First());

    }
}
