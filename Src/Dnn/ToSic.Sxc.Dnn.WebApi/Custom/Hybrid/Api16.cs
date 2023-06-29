using Custom.Hybrid.Advanced;
using System.Collections.Generic;
using System;
using ToSic.Eav;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Services;
using ToSic.Sxc.WebApi;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic WebAPI files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [PublicApi]
    [DnnLogExceptions]
    // v16 should now default to normal
    //[DefaultToNewtonsoftForHttpJson]
    public abstract class Api16: Api14<dynamic, ServiceKit14>,
        IDynamicCode12, 
        IDynamicWebApi, 
        IHasDynamicCodeRoot,
        IDynamicCode16
    {
        protected Api16() : base("Hyb14") { }

        protected Api16(string logSuffix) : base(logSuffix) { }

        #region Killed Properties from base class

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

        #endregion

        private CodeInfoService CcS => _ccs.Get(GetService<CodeInfoService>);
        private readonly GetOnce<CodeInfoService> _ccs = new GetOnce<CodeInfoService>();

        ///// <inheritdoc />
        //public new ITypedStack Settings => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Settings);

        ///// <inheritdoc />
        //public new ITypedStack Resources => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Resources);

        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)base.App;

        /// <inheritdoc />
        public ITypedStack SettingsStack => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public ITypedStack ResourcesStack => _DynCodeRoot.Resources;

        /// <inheritdoc />
        public ITypedStack SysSettings => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public ITypedStack SysResources => _DynCodeRoot.Resources;

        #endregion

        #region My... Stuff

        private TypedCode16Helper CodeHelper => _codeHelper ?? (_codeHelper = CreateCodeHelper());
        private TypedCode16Helper _codeHelper;

        private TypedCode16Helper CreateCodeHelper()
        {
            return new TypedCode16Helper(_DynCodeRoot, Data, null, false, "c# WebApiController");
        }

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        public IMyData MyData => _DynCodeRoot.Data as IMyData;

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

        #endregion

        public ITypedModel MyModel => CodeHelper.MyModel;

        /// <inheritdoc />
        public ITypedRead Read(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

    }
}
