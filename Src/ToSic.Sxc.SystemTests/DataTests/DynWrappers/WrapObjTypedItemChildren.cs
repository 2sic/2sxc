using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjTypedItemChildren(DynAndTypedTestHelper helper)
{

    private class TestData
    {
        public TestPerson Author { get; } = new()
        {
            Id = 101,
            Title = "some author",
            FirstName = "Some"
        };

        public TestPerson[] Readers { get; } =
        [
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
            }
        ];
    }
    private static readonly TestData Data = new();
    private ITypedItem Item => helper.Obj2Item(Data);
    private ITypedItem Author() => Item.Child("author");
    private ITypedItem Reader() => Item.Child("readers");
    private IEnumerable<ITypedItem> Readers() => Item.Children("readers");

    [Fact] public void ChildSingleExists() => NotNull(Author());
    [Fact] public void ChildDummyNotExists() => Null(Item.Child("dummy"));
    [Fact] public void ChildSingleId() => Equal(Data.Author.Id, Author().Id);
    [Fact] public void ChildSingleTitle() => Equal(Data.Author.Title, Author().Title);
    [Fact] public void ChildSingleFirstName() => Equal(Data.Author.FirstName, Author().String("firstname"));

    [Fact] public void ReaderSingleExists() => NotNull(Reader());
    [Fact] public void ReaderSingleId() => Equal(Data.Readers[0].Id, Reader().Id);

    [Fact] public void ReadersListExists() => NotNull(Readers());

    [Fact] public void ReadersListCount() => Equal(Data.Readers.Length, Readers().Count());
    [Fact] public void ReadersPersonCount() => Equal(Data.Readers.Length - 1, Item.Children("readers", type: "Person").Count());
    [Fact] public void ReadersCompanyCount() => Equal(1, Item.Children("readers", type: "Company").Count());
    [Fact] public void FakeListNotExists() => NotNull(Item.Children("dummy"));
    [Fact] public void ReadersList1Exists() => NotNull(Readers().First());
    [Fact] public void ReadersList1Id() => Equal(Data.Readers[0].Id, Readers().First().Id);
    [Fact] public void ReadersList2Exists() => NotNull(Readers().Skip(1).First());

}