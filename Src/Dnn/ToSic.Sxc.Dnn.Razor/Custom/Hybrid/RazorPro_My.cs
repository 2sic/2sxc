using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;
using static System.StringComparer;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class RazorPro: ISetDynamicModel
    {
        #region My... Stuff

        private TypedCode16Helper CodeHelper => _codeHelper ?? (_codeHelper = CreateCodeHelper());
        private TypedCode16Helper _codeHelper;

        void ISetDynamicModel.SetDynamicModel(object data) => _overridePageData = data;

        private object _overridePageData;

        private TypedCode16Helper CreateCodeHelper()
        {
            var myModelData = _overridePageData?.ObjectToDictionaryInvariant()
                              ?? PageData?
                                  .Where(pair => pair.Key is string)
                                  .ToDictionary(pair => pair.Key.ToString(), pair => pair.Value, InvariantCultureIgnoreCase);

            return new TypedCode16Helper(_DynCodeRoot, _DynCodeRoot.Data, myModelData, false, Path);
        }

        /// <inheritdoc />
        public ITypedItem MyItem => CodeHelper.MyItem;

        /// <inheritdoc />
        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        /// <inheritdoc />
        public ITypedItem MyHeader => CodeHelper.MyHeader;

        /// <inheritdoc />
        public IMyData MyData => _DynCodeRoot.Data as IMyData;

        #endregion



        #region MyModel


        /// <inheritdoc />
        public ITypedModel MyModel => CodeHelper.MyModel;

        #endregion

    }


}
