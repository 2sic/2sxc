using ToSic.Eav.Serialization;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using static System.Text.Json.JsonSerializer;

namespace ToSic.Sxc.Tests.DataTests
{
    public class DynAndTypedTestsBase: TestBaseSxcDb
    {
        protected DynAndTypedTestsBase()
        {
            // Wrapper = GetService<CodeDataWrapper>();
        }

        public CodeDataFactory Factory => _fac.Get(GetService<CodeDataFactory>);
        private readonly GetOnce<CodeDataFactory> _fac = new GetOnce<CodeDataFactory>();

        public CodeDataWrapper Wrapper => _wrapFac.Get(GetService<CodeDataWrapper>);
        private readonly GetOnce<CodeDataWrapper> _wrapFac = new GetOnce<CodeDataWrapper>();

        public DynamicJacketBase Json2Jacket(string jsonString) => Wrapper.FromJson(jsonString);

        public dynamic Json2Dyn(string jsonString) => Json2Jacket(jsonString);

        public dynamic Json2Obj2Dyn(object obj) => Json2Jacket(JsonSerialize(obj));

        public string JsonSerialize(object obj) => Serialize(obj, JsonOptions.UnsafeJsonWithoutEncodingHtml);

        public (dynamic Dyn, string Json, T Original) DynJsonAndOriginal<T>(T original)
        {
            var json = JsonSerialize(original);
            return (Json2Dyn(json), json, original);
        }


        public WrapObjectDynamic WrapObjFromObject(object data, bool wrapChildren = true, bool realObjectsToo = true)
            => Wrapper.FromObject(data, WrapperSettings.Dyn(children: wrapChildren, realObjectsToo: realObjectsToo));

        public dynamic DynFromWrapFromObject(object data) => WrapObjFromObject(data);


        public ITyped Obj2Typed(object data, WrapperSettings? reWrap = null)
            => Wrapper.TypedFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

        public ITyped Obj2Json2Typed(object data, WrapperSettings? settings = null)
        {
            return Json2Jacket(JsonSerialize(data));
        }

        public ITypedItem Obj2Item(object data, WrapperSettings? reWrap = null)
            => Wrapper.TypedItemFromObject(data, reWrap ?? WrapperSettings.Typed(true, true));

    }
}
