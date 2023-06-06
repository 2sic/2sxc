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

        [PrivateApi("WIP v16.02")]
        public ICodeParameters Parameters => _parameters.Get(() =>
        {
            if (_overridePageData != null)
                return new CodeParameters(_overridePageData.ObjectToDictionary(), _DynCodeRoot);

            var stringDic = PageData?
                .Where(pair => pair.Key is string)
                .ToDictionary(pair => pair.Key.ToString(), pair => pair.Value, InvariantCultureIgnoreCase);
            return new CodeParameters(stringDic, _DynCodeRoot);
        });
        private readonly GetOnce<ICodeParameters> _parameters = new GetOnce<ICodeParameters>();
        private object _overridePageData;
    }
}
