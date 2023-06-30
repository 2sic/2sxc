using Custom.Hybrid.Advanced;
using System;
using System.Collections.Generic;
using ToSic.Eav;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    // ReSharper disable once UnusedMember.Global
    public abstract class Razor16: Razor14<dynamic, ServiceKit14>, IDynamicCode16
    {
        private CodeInfoService CcS => _ccs.Get(GetService<CodeInfoService>);
        private readonly GetOnce<CodeInfoService> _ccs = new();

        ///// <inheritdoc />
        //public new ITypedStack Settings => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Settings);

        ///// <inheritdoc />
        //public new ITypedStack Resources => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Resources);

        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)base.App;

        [PrivateApi] public ITypedStack ResourcesStack => _DynCodeRoot.Resources;
        [PrivateApi] public ITypedStack SettingsStack => _DynCodeRoot.Settings;

        /// <inheritdoc cref="IDynamicCode16.AllResources" />
        public ITypedStack AllResources => _DynCodeRoot.Resources;

        /// <inheritdoc cref="IDynamicCode16.AllSettings" />
        public ITypedStack AllSettings => _DynCodeRoot.Settings;

        #endregion

        #region My... Stuff

        private TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
        private TypedCode16Helper _codeHelper;

        private TypedCode16Helper CreateCodeHelper()
        {
            var myModelData = _overridePageData?.ObjectToDictionaryInvariant()
                      ?? Model?.ObjectToDictionary();
            return new TypedCode16Helper(_DynCodeRoot, Data, myModelData, true, Path);
        }

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        public IMyData MyData => _DynCodeRoot.Data as IMyData;

        #endregion


        #region Killed Properties from base class

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object DynamicModel => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use TypedModel instead.");

        #endregion

        #region AsItem(s) / Merge

        /// <inheritdoc />
        public ITypedStack Merge(params object[] items) => _DynCodeRoot.AsC.AsStack(items);

        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.AsC.AsStack(items);


        /// <inheritdoc />
        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.AsC.AsEntity(thing);

        #endregion

        #region MyModel

        [PrivateApi("WIP 16.02 - to be removed")]
        public ITypedModel TypedModel => CcS.GetAndWarn(DynamicCode16Warnings.NoTypedModel, MyModel);

        [PrivateApi("WIP v16.02")]
        public ITypedModel MyModel => CodeHelper.MyModel;

        internal override void UpdateModel(object data) => _overridePageData = data;
        private object _overridePageData;


        #endregion

        /// <inheritdoc />
        public ITypedRead Read(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

    }
}
