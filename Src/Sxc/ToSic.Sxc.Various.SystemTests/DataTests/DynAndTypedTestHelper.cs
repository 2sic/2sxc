using ToSic.Eav.Serialization;
using ToSic.Lib.DI;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Internal.Dynamic;
using ToSic.Sxc.Data.Internal.Wrapper;
using static System.Text.Json.JsonSerializer;

namespace ToSic.Sxc.DataTests;


public class DynAndTypedTestHelper(ICodeDataFactory factory, ICodeDataPoCoWrapperService wrapper, Generator<CodeJsonWrapper> codeJsonGenerator)
{
    #region Helper / Factories

    public ICodeDataFactory Factory => factory;

    public ICodeDataPoCoWrapperService Wrapper => wrapper;

    public CodeJsonWrapper JsonWrapper => codeJsonGenerator.New();

    #endregion

    public object Json2Jacket(string jsonString) => Factory.Json2Jacket(jsonString);

    public dynamic Json2Dyn(string jsonString) => Json2Jacket(jsonString);

    public dynamic Obj2Json2Dyn(object obj) => Json2Jacket(JsonSerialize(obj));

    public string JsonSerialize(object obj) => Serialize(obj, JsonOptions.UnsafeJsonWithoutEncodingHtml);

    public (dynamic Dyn, string Json, T Original) DynJsonAndOriginal<T>(T original)
    {
        var json = JsonSerialize(original);
        return (Json2Dyn(json), json, original);
    }


    public  object Obj2WrapObj(object data, bool wrapChildren = true, bool realObjectsToo = true)
        => Wrapper.DynamicFromObject(data, WrapperSettings.Dyn(children: wrapChildren, realObjectsToo: realObjectsToo));

    public dynamic Obj2WrapObjAsDyn(object data) => Obj2WrapObj(data);

    public ITypedItem Obj2Item(object data, WrapperSettings? reWrap = null)
        => Wrapper.TypedItemFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

    #region To Json Wrappers

    public ITyped Obj2Typed(object data, WrapperSettings? reWrap = null)
        => Wrapper.TypedFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

    public ITyped Obj2Json2TypedStrict(object data)
        => Obj2Json2Typed(data, WrapperSettings.Typed(true, true, propsRequired: true));

    public ITyped Obj2Json2TypedLoose(object data)
        => Obj2Json2Typed(data, WrapperSettings.Typed(true, true, propsRequired: false));

    private ITyped Obj2Json2Typed(object data, WrapperSettings settings) 
        => JsonWrapper.Setup(settings).JsonToTyped(JsonSerialize(data));


    public IEnumerable<ITyped> Obj2Json2TypedListStrict(object data)
        => Obj2Json2TypedList(data, WrapperSettings.Typed(true, true, propsRequired: true));

    private IEnumerable<ITyped> Obj2Json2TypedList(object data, WrapperSettings settings) 
        => JsonWrapper.Setup(settings).JsonToTypedList(JsonSerialize(data));
    #endregion


}