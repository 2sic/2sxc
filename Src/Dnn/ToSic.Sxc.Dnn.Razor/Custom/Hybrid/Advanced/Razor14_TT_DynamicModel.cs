using System.Dynamic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;
using static System.StringComparer;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    public abstract partial class Razor14<TModel, TServiceKit>: ISetDynamicModel
    {
        /// <summary>
        /// Dynamic object containing parameters. So in Dnn it contains the PageData, in Oqtane it contains the Model
        /// </summary>
        /// <remarks>
        /// New in v12
        /// </remarks>
        public dynamic DynamicModel => _dynamicModel ?? (_dynamicModel = new DynamicReadDictionary<object, dynamic>(PageData));
        private DynamicObject _dynamicModel;

        void ISetDynamicModel.SetDynamicModel(object data)
        {
            _overridePageData = data;
            _dynamicModel = new DynamicReadObject(data, false, false);
        }

        // TODO: MOVE TO V16
        [PrivateApi("WIP v16.02")]
        public ITypedModel TypedModel => _typedModel.Get(() =>
        {
            if (_overridePageData != null)
                return new TypedModel(_overridePageData.ObjectToDictionary(), _DynCodeRoot, this.Path);

            var stringDic = PageData?
                .Where(pair => pair.Key is string)
                .ToDictionary(pair => pair.Key.ToString(), pair => pair.Value, InvariantCultureIgnoreCase);
            return new TypedModel(stringDic, _DynCodeRoot, this.Path);
        });
        private readonly GetOnce<ITypedModel> _typedModel = new GetOnce<ITypedModel>();
        private object _overridePageData;
    }
}
