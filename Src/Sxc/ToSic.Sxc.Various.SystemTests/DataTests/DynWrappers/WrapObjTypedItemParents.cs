using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjTypedItemParents(DynAndTypedTestHelper helper)
{
    private class TestTag
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Field { get; set; }
        public virtual string Type => "Tag";
    }

    private class TestCategory : TestTag
    {
        public override string Type => "Category";
    }
    private static readonly TestTag[] TestTagList =
    [
        new()
        {
            Id = 1001,
            Title = "Web",
            Field = "Tags",
        },
        new()
        {
            Id = 1002,
            Title = "IT",
            Field = "Advanced"
        },
        new TestCategory
        {
            Id = 2001,
            Title = "Cat1",
            Field = "Advanced"
        },
        new TestCategory
        {
            Id = 2002,
            Title = "Cat2",
            Field = "Advanced"
        }
    ];


    private class TestDataParents
    {
        public TestTag[] Parents => TestTagList;
    }
    private static readonly TestDataParents DataParent = new();
    private ITypedItem Item => helper.Obj2Item(DataParent);

    private IEnumerable<ITypedItem> TagsFlat => Item.Parents();
    private IEnumerable<ITypedItem> TagsFlatInField(string field) => Item.Parents(field: field);

    [Fact] public void ParentsFlatExists() => NotNull(TagsFlat);
    [Fact] public void ParentsFlatCount() => Equal(4, TagsFlat.Count());
    [Fact] public void ParentsFlat1Id() => Equal(DataParent.Parents[0].Id, TagsFlat.First().Id);
    [Fact] public void ParentsFlat1Name() => Equal(DataParent.Parents[0].Title, TagsFlat.First().Title);
    [Fact] public void ParentsFlat2Id() => Equal(DataParent.Parents[1].Id, TagsFlat.Skip(1).First().Id);
    [Fact] public void ParentsFlatTagsCount() => Equal(1, TagsFlatInField("Tags").Count());
    [Fact] public void ParentsFlatCategoriesCount() => Equal(3, TagsFlatInField("Advanced").Count());

        
    /// <summary> Only the ones of type "Tag" </summary>
    private IEnumerable<ITypedItem> Tags => Item.Parents(type: "Tag");
    private IEnumerable<ITypedItem> TypeFake => Item.Parents(type: "Fake");
    [Fact] public void ParentTagsExists() => NotNull(Tags);
    [Fact] public void ParentTagsExists2() => Equal(2, Tags.Count());
    [Fact] public void ParentFakeEmptyList() => NotNull(TypeFake);
    [Fact] public void ParentFakeEmptyList0() => Equal(0, TypeFake.Count());

    [Fact] public void ParentTagsOfField() => Equal(1, Item.Parents(type: "Tag", field: "Tags").Count());
        
}