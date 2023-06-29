using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;
using CodeInfoService = ToSic.Eav.Code.InfoSystem.CodeInfoService;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v16 Dynamic Code files.
    /// 
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14/16, so that your code can be lighter. 
    /// </summary>
    [WorkInProgressApi("WIP 16.02")]
    public abstract class Code16 : DynamicCodeBase, IHasCodeLog, IDynamicCode16
    {

        #region Constructor / Setup

        /// <summary>
        /// Main constructor. May never have parameters, otherwise inheriting code will run into problems. 
        /// </summary>
        protected Code16() : base("Sxc.Code14") { }

        // <inheritdoc />
        public new ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc cref="IDynamicCode12.GetService{TService}" />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        private TypedCode16Helper CodeHelper => _codeHelper ?? (_codeHelper = CreateCodeHelper());
        private TypedCode16Helper _codeHelper;

        #endregion

        public ServiceKit14 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _kit = new GetOnce<ServiceKit14>();

        #region Stuff added by Code12

        ///// <inheritdoc cref="IDynamicCode12.Convert" />
        //public IConvertService Convert => _DynCodeRoot.Convert;

        ///// <inheritdoc cref="IDynamicCode12.Resources" />
        //public dynamic Resources => _DynCodeRoot?.Resources;

        ///// <inheritdoc cref="IDynamicCode12.Settings" />
        //public dynamic Settings => _DynCodeRoot?.Settings;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion


        #region Link and Edit
        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;
        ///// <inheritdoc cref="IDynamicCode.Edit" />
        //public IEditService Edit => _DynCodeRoot?.Edit;

        #endregion


        #region SharedCode - must also map previous path to use here

        /// <inheritdoc />
        [PrivateApi]
        public string CreateInstancePath { get; set; }

        /// <inheritdoc cref="IDynamicCode.CreateInstance" />
        public dynamic CreateInstance(string virtualPath, string noParamOrder = Protector, string name = null, string relativePath = null, bool throwOnError = true) =>
            SysHlp.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

        #endregion


        #region Context, Settings, Resources - TODO: KILL

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        public ICmsContext CmsContext => CcS.GetAndWarn(DynamicCode16Warnings.NoCmsContext, _DynCodeRoot?.CmsContext);

        #endregion CmsContext

        #region AsDynamic and AsEntity

        ///// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
        //public dynamic AsDynamic(string json, string fallback = default) => _DynCodeRoot?.AsC.AsDynamicFromJson(json, fallback);

        ///// <inheritdoc cref="IDynamicCode.AsDynamic(IEntity)" />
        //public dynamic AsDynamic(IEntity entity) => _DynCodeRoot?.AsC.AsDynamic(entity);

        ///// <inheritdoc cref="IDynamicCode.AsDynamic(object)" />
        //public dynamic AsDynamic(object dynamicEntity) => _DynCodeRoot?.AsC.AsDynamicInternal(dynamicEntity);

        ///// <inheritdoc cref="IDynamicCode12.AsDynamic(object[])" />
        //public dynamic AsDynamic(params object[] entities) => _DynCodeRoot?.AsC.MergeDynamic(entities);

        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        public IEntity AsEntity(object dynamicEntity) => _DynCodeRoot?.AsC.AsEntity(dynamicEntity);

        #endregion




        #region Killed Properties from base class

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Content => throw new NotSupportedException($"{nameof(Content)} isn't supported in v16 typed. Use Data.MyContent instead.");

        [PrivateApi("Hide as it's nothing that should be used")]
        public new object Header => throw new NotSupportedException($"{nameof(Header)} isn't supported in v16 typed. Use Data.MyHeader instead.");

        #endregion

        private CodeInfoService CcS => _ccs.Get(GetService<CodeInfoService>);
        private readonly GetOnce<CodeInfoService> _ccs = new GetOnce<CodeInfoService>();

        ///// <inheritdoc />
        //public ITypedStack Settings => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Settings);

        ///// <inheritdoc />
        //public ITypedStack Resources => CcS.GetAndWarn(DynamicCode16Warnings.AvoidSettingsResources, _DynCodeRoot.Resources);

        #region New App, Settings, Resources

        /// <inheritdoc />
        public IAppTyped App => (IAppTyped)_DynCodeRoot?.App;

        /// <inheritdoc />
        public ITypedStack SettingsStack => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public ITypedStack ResourcesStack => _DynCodeRoot.Resources;

        /// <inheritdoc />
        public ITypedStack SysSettings => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public ITypedStack SysResources => _DynCodeRoot.Resources;

        public IMyData MyData => _DynCodeRoot.Data as IMyData;

        #endregion

        #region My... Stuff


        private TypedCode16Helper CreateCodeHelper()
        {
            return new TypedCode16Helper(_DynCodeRoot, MyData, null, false, "c# code file");
        }

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        #endregion


        #region AsItem(s) / Merge

        /// <inheritdoc />
        public ITypedStack Merge(params object[] items) => _DynCodeRoot.AsC.AsStack(items);
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.AsC.AsStack(items);

        /// <inheritdoc />
        public ITypedItem AsItem(object target, string noParamOrder = Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        #endregion

        public ITypedModel MyModel => CodeHelper.MyModel;

        public ICmsContext MyContext => CmsContext;

        /// <inheritdoc />
        public ITypedRead Read(string json, string fallback = default) => _DynCodeRoot.AsC.AsDynamicFromJson(json, fallback);

    }
}
