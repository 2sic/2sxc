using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjTypedItemMetadata(DynAndTypedTestHelper helper)
{
    private class TestDataMd1
    {
        public dynamic Metadata => new
        {
            Id = 999,
            Description = "MD Description"
        };
    }
    private ITypedItem ItemMd1 => helper.Obj2Item(new TestDataMd1());

    [Fact] public void MetadataHasValue() => NotNull(ItemMd1.TestMetadata());
    [Fact] public void MetadataCount1() => Equal(1, ((ItemMd1.TestMetadata() as IHasMetadata).Metadata as IEnumerable<IEntity>).Count());
    //[Fact] public void MetadataId() => Equal(999, Item.Metadata.Entity.EntityId);
    [Fact] public void MetadataDescription() => Equal("MD Description", ItemMd1.TestMetadata().Get<string>("Description"));

    private class TestDataMd3
    {
        public dynamic Metadata => new object[]
        {
            new
            {
                Id = 999,
                Description = "MD3 Description"
            },
            new {
                Id = 555,
                Color = "white"
            },
            new {
                Id = 555,
                Color = "black"
            },
        };

    }
    private ITypedItem ItemMd3 => helper.Obj2Item(new TestDataMd3());
    [Fact] public void Metadata3HasValue() => NotNull(ItemMd3.TestMetadata());
    [Fact] public void Metadata3Count3() => Equal(3, ((ItemMd3.TestMetadata() as IHasMetadata).Metadata /*as IEnumerable<IDynamicEntity>*/).Count());
    [Fact] public void Metadata3Description() => Equal("MD3 Description", ItemMd3.TestMetadata().Get<string>("Description"));
    [Fact] public void Metadata3Color() => Equal("white", ItemMd3.TestMetadata().Get<string>("Color"));

}