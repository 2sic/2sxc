namespace ToSic.Sxc.DataTests.DynWrappers;


public class WrapObjTypedItem(DynAndTypedTestHelper helper)
{

    [Fact]
    public void PropsWhichDefaultToNull()
    {
        var anon = new
        {
            Simple = "simple",
            Sub = new
            {
                SubSub = "Test"
            }
        };

        var typed = helper.Obj2Item(anon);
        Null(typed.Entity);
        Null(typed.Type);
        Null(typed.Presentation);
        Null(typed.Child("DoesntExist"));
    }

    [Fact] public void IsDemoItemDefault() => False(helper.Obj2Item(new { }).IsDemoItem);
    [Fact] public void IsDemoItemFalse() => False(helper.Obj2Item(new { IsDemoItem = false }).IsDemoItem);
    [Fact] public void IsDemoItemTrue() => True(helper.Obj2Item(new { IsDemoItem = true }).IsDemoItem);

    [Fact] public void IdDefault() => Equal(0, helper.Obj2Item(new { }).Id);
    [Fact] public void Id27() => Equal(27, helper.Obj2Item(new { Id = 27 }).Id);
    [Fact] public void Id27Case() => Equal(27, helper.Obj2Item(new { ID = 27 }).Id);

    [Fact] public void GuidDefault() => Equal(Guid.Empty, helper.Obj2Item(new { }).Guid);
    [Fact] public void GuidCustom() => Equal(_guid, helper.Obj2Item(new { Guid = _guid }).Guid);
    private static readonly Guid _guid = Guid.NewGuid();
    [Fact] public void TitleDefault() => Null(helper.Obj2Item(new { }).Title);
    [Fact] public void TitleCustom() => Equal("title X", helper.Obj2Item(new { Title = "title X" }).Title);
    [Fact] public void TitleNumber() => Equal("222", helper.Obj2Item(new { Title = 222 }).Title);
        
}