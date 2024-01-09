using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Razor.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using static System.StringComparer;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// Base class for v14 Dynamic Razor files.
/// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
/// This contains all the popular services used in v14, so that your code can be lighter. 
/// </summary>
/// <remarks>
/// Important: This is very different from Razor12 or Razor14, as it doesn't rely on `dynamic` code any more.
/// Be aware of this since the APIs are very different.
/// </remarks>
[PublicApi]
public abstract class RazorTyped: RazorComponentBase, IRazor, IDynamicCode16, IHasCodeHelp, IGetCodePath, ISetDynamicModel, ICanUseRoslynCompiler
{
    #region Constructor, Setup, Helpers

    /// <inheritdoc cref="DnnRazorHelper.RenderPageNotSupported"/>
    [PrivateApi]
    public override HelperResult RenderPage(string path, params object[] data)
        => SysHlp.RenderPageNotSupported();


    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel16;

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => _DynCodeRoot.GetService<TService>();


    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    public ServiceKit16 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit16>());
    private readonly GetOnce<ServiceKit16> _kit = new();

    private TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
    private TypedCode16Helper _codeHelper;

    void ISetDynamicModel.SetDynamicModel(object data) => _overridePageData = data;

    private object _overridePageData;

    private TypedCode16Helper CreateCodeHelper()
    {
        var myModelData = _overridePageData?.ToDicInvariantInsensitive()
                          ?? PageData?
                              .Where(pair => pair.Key is string)
                              .ToDictionary(pair => pair.Key.ToString(), pair => pair.Value, InvariantCultureIgnoreCase);

        return new TypedCode16Helper(_DynCodeRoot, _DynCodeRoot.Data, myModelData, false, Path);
    }


    #endregion


    #region Core Properties which should appear in docs

    /// <inheritdoc />
    public override ICodeLog Log => SysHlp.CodeLog;

    /// <inheritdoc />
    public override IHtmlHelper Html => SysHlp.Html;

    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default) => SysHlp.GetCode(path, noParamOrder, className);

    #endregion

    #region Link, Edit

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _DynCodeRoot.Link;

    #endregion


    #region New App, Settings, Resources

    /// <inheritdoc />
    public new IAppTyped App => (IAppTyped)_DynCodeRoot.App;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion

    #region My Data Stuff

    /// <inheritdoc />
    public ITypedItem MyItem => CodeHelper.MyItem;

    /// <inheritdoc />
    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    /// <inheritdoc />
    public ITypedItem MyHeader => CodeHelper.MyHeader;

    /// <inheritdoc />
    public IBlockRun MyData => _DynCodeRoot.Data;

    /// <inheritdoc />
    public ITypedModel MyModel => CodeHelper.MyModel;

    #endregion


    #region MyContext & UniqueKey

    /// <inheritdoc cref="IDynamicCode16.MyContext" />
    public ICmsContext MyContext => _DynCodeRoot.CmsContext;

    /// <inheritdoc cref="IDynamicCode16.MyPage" />
    public ICmsPage MyPage => _DynCodeRoot.CmsContext.Page;

    /// <inheritdoc cref="IDynamicCode16.MyUser" />
    public ICmsUser MyUser => _DynCodeRoot.CmsContext.User;

    /// <inheritdoc cref="IDynamicCode16.MyView" />
    public ICmsView MyView => _DynCodeRoot.CmsContext.View;

    /// <inheritdoc cref="IDynamicCode16.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion

    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => _DynCodeRoot.Cdf.AsItem(data, propsRequired: propsRequired ?? true, mock: mock);

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default) 
        => _DynCodeRoot.Cdf.AsItems(list, propsRequired: propsRequired ?? true);

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing) => _DynCodeRoot.Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _DynCodeRoot.Cdf.AsTyped(original, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _DynCodeRoot.Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items) => _DynCodeRoot.Cdf.AsStack(items);

    #endregion


    #region Dev Tools & Dev Helpers

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => CodeHelper.DevTools;

    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => CodeHelpDbV16.Compile16;

    #endregion

    #region CreateInstance

    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    #endregion

    void ICanUseRoslynCompiler.AttachRazorEngine(DnnRazorEngine razorEngine)
    {
        if (_razorEngine == null) _razorEngine = razorEngine;
    }
    private DnnRazorEngine _razorEngine;

    public HelperResult RoslynRenderPage(string virtualPath, object data)
    {
        // TODO: switch based on razorType.ThisApp
        //return base.RenderPage(virtualPath, data);
        return _razorEngine?.RenderPage(this.NormalizePath(virtualPath), data);
    }
}