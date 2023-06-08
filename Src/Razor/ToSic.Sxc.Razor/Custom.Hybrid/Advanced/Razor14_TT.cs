using System.Collections.Generic;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using static System.StringComparer;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Razor14<TModel, TServiceKit>: Razor12<TModel>, IRazor14<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly GetOnce<TServiceKit> _kit = new();


        /// <inheritdoc />
        public ITypedItem AsTyped(object target, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsTyped(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsTypedList(object list, string noParamOrder = ToSic.Eav.Parameters.Protector) => _DynCodeRoot.AsTypedList(list);


        internal override void UpdateModel(object data) => _overridePageData = data;
        private object _overridePageData;

        [PrivateApi("WIP v16.02")]
        public ITypedModel TypedModel => _parameters.Get(() =>
        {
            if (_overridePageData != null)
                return new TypedModel(_overridePageData.ObjectToDictionary(), _DynCodeRoot, Path);

            var stringDic = Model?.ObjectToDictionary() ?? new Dictionary<string, object>(InvariantCultureIgnoreCase);
            return new TypedModel(stringDic, _DynCodeRoot, Path);
        });
        private readonly GetOnce<ITypedModel> _parameters = new();


    }
}
