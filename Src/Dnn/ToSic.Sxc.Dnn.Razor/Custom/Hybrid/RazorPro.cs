using System.Collections.Generic;
using System.Web.WebPages;
using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using Constants = ToSic.Sxc.Constants;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic Razor files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: This is very different from Razor12 or Razor14, as it doesn't rely on `dynamic` code any more.
    /// Be aware of this since the APIs are very different.
    /// </remarks>
    [WorkInProgressApi("WIP 16.02 - not final")]
    public abstract partial class RazorPro: RazorComponentBase, IRazor, IDynamicCode16, IHasCodeHelp, IGetCodePath
    {

        /// <inheritdoc cref="RazorHelper.RenderPageNotSupported"/>
        [PrivateApi]
        public override HelperResult RenderPage(string path, params object[] data)
            => SysHlp.RenderPageNotSupported();


        [PrivateApi] public override int CompatibilityLevel => Constants.CompatibilityLevel16;

        /// <inheritdoc />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();


        public ServiceKit14 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _kit = new GetOnce<ServiceKit14>();

        #region Core Properties which should appear in docs

        /// <inheritdoc />
        public override ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc />
        public override IHtmlHelper Html => SysHlp.Html;

        /// <inheritdoc cref="IDynamicCode16.GetCode"/>
        public dynamic GetCode(string path) => SysHlp.CreateInstance(path);

        #endregion

        #region Link, Edit

        /// <inheritdoc />
        public ILinkService Link => _DynCodeRoot.Link;

        #endregion


        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)_DynCodeRoot.App;

        /// <inheritdoc cref="IDynamicCode16.AllResources" />
        public ITypedStack AllResources => _DynCodeRoot.Resources;

        /// <inheritdoc cref="IDynamicCode16.AllSettings" />
        public ITypedStack AllSettings => _DynCodeRoot.Settings;

        #endregion


        #region MyContext

        /// <inheritdoc />
        public ICmsContext MyContext => _DynCodeRoot.CmsContext;

        /// <inheritdoc />
        public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

        /// <inheritdoc />
        public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

        /// <inheritdoc />
        public ICmsView MyView => _DynCodeRoot.CmsContext.View;

        #endregion

        #region As Conversions

        /// <inheritdoc cref="IDynamicCode16.AsItem" />
        public ITypedItem AsItem(object target, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItem(target);

        /// <inheritdoc cref="IDynamicCode16.AsItems" />
        public IEnumerable<ITypedItem> AsItems(object list, string noParamOrder = Parameters.Protector)
            => _DynCodeRoot.AsC.AsItems(list);

        /// <inheritdoc cref="IDynamicCode16.AsEntity" />
        public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.AsC.AsEntity(thing);

        /// <inheritdoc cref="IDynamicCode16.AsTyped" />
        public ITyped AsTyped(object original) => _DynCodeRoot.AsC.AsTyped(original);

        /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
        public IEnumerable<ITyped> AsTypedList(object list) => _DynCodeRoot.AsC.AsTypedList(list);

        /// <inheritdoc cref="IDynamicCode16.AsStack" />
        public ITypedStack AsStack(params object[] items) => _DynCodeRoot.AsC.AsStack(items);

        #endregion


        #region Dev Tools & Dev Helpers

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => CodeHelpDbV16.Compile16;

        #endregion

        #region CreateInstance

        [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

        #endregion

    }


}
