using ToSic.Eav.Serialization;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using static System.Text.Json.JsonSerializer;

namespace ToSic.Sxc.Tests.DataTests
{
    public class DynAndTypedTestsBase: TestBaseSxcDb
    {
        #region Helper / Factories

        public CodeDataFactory Factory => _fac.Get(GetService<CodeDataFactory>);
        private readonly GetOnce<CodeDataFactory> _fac = new GetOnce<CodeDataFactory>();

        public CodeDataWrapper Wrapper => _wrapFac.Get(GetService<CodeDataWrapper>);
        private readonly GetOnce<CodeDataWrapper> _wrapFac = new GetOnce<CodeDataWrapper>();

        public CodeJsonWrapper JsonWrapper => _codeJson.Get(GetService<Generator<CodeJsonWrapper>>).New();
        private readonly GetOnce<Generator<CodeJsonWrapper>> _codeJson = new GetOnce<Generator<CodeJsonWrapper>>();

        #endregion

        public DynamicJacketBase Json2Jacket(string jsonString) => Factory.Json2Jacket(jsonString);

        public dynamic Json2Dyn(string jsonString) => Json2Jacket(jsonString);

        public dynamic Obj2Json2Dyn(object obj) => Json2Jacket(JsonSerialize(obj));

        public string JsonSerialize(object obj) => Serialize(obj, JsonOptions.UnsafeJsonWithoutEncodingHtml);

        public (dynamic Dyn, string Json, T Original) DynJsonAndOriginal<T>(T original)
        {
            var json = JsonSerialize(original);
            return (Json2Dyn(json), json, original);
        }


        public WrapObjectDynamic Obj2WrapObj(object data, bool wrapChildren = true, bool realObjectsToo = true)
            => Wrapper.FromObject(data, WrapperSettings.Dyn(children: wrapChildren, realObjectsToo: realObjectsToo));

        public dynamic Obj2WrapObjAsDyn(object data) => Obj2WrapObj(data);


        public ITyped Obj2Typed(object data, WrapperSettings? reWrap = null)
            => Wrapper.TypedFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

        public ITyped Obj2Json2TypedStrict(object data)
            => Obj2Json2Typed(data, WrapperSettings.Typed(true, true, propsRequired: true));

        public ITyped Obj2Json2TypedLoose(object data)
            => Obj2Json2Typed(data, WrapperSettings.Typed(true, true, propsRequired: false));

        private ITyped Obj2Json2Typed(object data, WrapperSettings settings) 
            => JsonWrapper.Setup(settings).Json2Typed(JsonSerialize(data));

        public ITypedItem Obj2Item(object data, WrapperSettings? reWrap = null)
            => Wrapper.TypedItemFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

    }
}
