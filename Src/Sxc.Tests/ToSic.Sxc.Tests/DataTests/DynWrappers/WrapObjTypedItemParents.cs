using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests.DynWrappers
{
    [TestClass]
    public class WrapObjTypedItemParents : DynAndTypedTestsBase
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
        {
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
        };


        private class TestDataParents
        {
            public TestTag[] Parents => TestTagList;
        }
        private static readonly TestDataParents DataParent = new();
        private ITypedItem Item => Obj2Item(DataParent);

        private IEnumerable<ITypedItem> TagsFlat => Item.Parents();
        private IEnumerable<ITypedItem> TagsFlatInField(string field) => Item.Parents(field: field);

        [TestMethod] public void ParentsFlatExists() => IsNotNull(TagsFlat);
        [TestMethod] public void ParentsFlatCount() => AreEqual(4, TagsFlat.Count());
        [TestMethod] public void ParentsFlat1Id() => AreEqual(DataParent.Parents[0].Id, TagsFlat.First().Id);
        [TestMethod] public void ParentsFlat1Name() => AreEqual(DataParent.Parents[0].Title, TagsFlat.First().Title);
        [TestMethod] public void ParentsFlat2Id() => AreEqual(DataParent.Parents[1].Id, TagsFlat.Skip(1).First().Id);
        [TestMethod] public void ParentsFlatTagsCount() => AreEqual(1, TagsFlatInField("Tags").Count());
        [TestMethod] public void ParentsFlatCategoriesCount() => AreEqual(3, TagsFlatInField("Advanced").Count());

        
        /// <summary> Only the ones of type "Tag" </summary>
        private IEnumerable<ITypedItem> Tags => Item.Parents(type: "Tag");
        private IEnumerable<ITypedItem> TypeFake => Item.Parents(type: "Fake");
        [TestMethod] public void ParentTagsExists() => IsNotNull(Tags);
        [TestMethod] public void ParentTagsExists2() => AreEqual(2, Tags.Count());
        [TestMethod] public void ParentFakeEmptyList() => IsNotNull(TypeFake);
        [TestMethod] public void ParentFakeEmptyList0() => AreEqual(0, TypeFake.Count());

        [TestMethod] public void ParentTagsOfField() => AreEqual(1, Item.Parents(type: "Tag", field: "Tags").Count());
        
    }
}
