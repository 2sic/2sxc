using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;
using Constants = ToSic.Sxc.Constants;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v16 Pro Dynamic Code files.
    /// </summary>
    [WorkInProgressApi("WIP 16.02")]
    public abstract class CodeTyped : DynamicCodeBase, IHasCodeLog, IDynamicCode16
    {

        #region Constructor / Setup

        /// <summary>
        /// Main constructor.
        /// Doesn't have parameters so it can easily be inherited.
        /// </summary>
        protected CodeTyped() : base("Sxc.Code16") { }

        /// <inheritdoc cref="IHasCodeLog.Log" />
        public new ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
        public TService GetService<TService>() where TService : class => _DynCodeRoot.GetService<TService>();

        private TypedCode16Helper CodeHelper 
            => _codeHelper ?? (_codeHelper = new TypedCode16Helper(_DynCodeRoot, MyData, null, false, "c# code file"));
        private TypedCode16Helper _codeHelper;

        [PrivateApi] public override int CompatibilityLevel => Constants.CompatibilityLevel16;

        #endregion

        /// <inheritdoc cref="IDynamicCode16.Kit"/>
        public ServiceKit16 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit16>());
        private readonly GetOnce<ServiceKit16> _kit = new GetOnce<ServiceKit16>();

        #region Stuff added by Code12

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => CodeHelper.DevTools;

        #endregion


        #region Link and Edit
        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot?.Link;

        #endregion


        #region SharedCode - must also map previous path to use here

        /// <inheritdoc />
        [PrivateApi]
        string IGetCodePath.CreateInstancePath { get; set; }

        /// <inheritdoc cref="IDynamicCode16.GetCode"/>
        public dynamic GetCode(string path, string noParamOrder = Protector, string className = default) => SysHlp.GetCode(path, noParamOrder, className);


        #endregion


        #region New App, Settings, Resources

        /// <inheritdoc />
        public IAppTyped App => (IAppTyped)_DynCodeRoot?.App;

        /// <inheritdoc cref="IDynamicCode16.AllResources" />
        public ITypedStack AllResources => CodeHelper.AllResources;

        /// <inheritdoc cref="IDynamicCode16.AllSettings" />
        public ITypedStack AllSettings => CodeHelper.AllSettings;


        public IContextData MyData => _DynCodeRoot.Data;

        #endregion

        #region My... Stuff

        public ITypedItem MyItem => CodeHelper.MyItem;

        public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

        public ITypedItem MyHeader => CodeHelper.MyHeader;

        #endregion


        #region As Conversions

        /// <inheritdoc cref="IDynamicCode16.AsItem" />
        public ITypedItem AsItem(object data, string noParamOrder = Protector, bool? propsRequired = default, bool? mock = default)
            => _DynCodeRoot.Cdf.AsItem(data, noParamOrder, propsRequired: propsRequired ?? true, mock: mock);

        /// <inheritdoc cref="IDynamicCode16.AsItems" />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsItems(list, noParamOrder, propsRequired: propsRequired ?? true);

        /// <inheritdoc cref="IDynamicCode16.AsEntity" />
        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.Cdf.AsEntity(thing);

        /// <inheritdoc cref="IDynamicCode16.AsTyped" />
        public ITyped AsTyped(object original, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsTyped(original, propsRequired: propsRequired);

        /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
        public IEnumerable<ITyped> AsTypedList(object list, string noParamOrder = Protector, bool? propsRequired = default)
            => _DynCodeRoot.Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

        /// <inheritdoc cref="IDynamicCode16.AsStack" />
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.Cdf.AsStack(items);

        #endregion

        public ITypedModel MyModel => CodeHelper.MyModel;

        #region MyContext

        /// <inheritdoc cref="IDynamicCode16.MyContext" />
        public ICmsContext MyContext => _DynCodeRoot.CmsContext;

        /// <inheritdoc cref="IDynamicCode16.MyUser" />
        public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

        /// <inheritdoc cref="IDynamicCode16.MyPage" />
        public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

        /// <inheritdoc cref="IDynamicCode16.MyView" />
        public ICmsView MyView => _DynCodeRoot.CmsContext.View;

        #endregion
    }
}
