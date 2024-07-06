using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Data;
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
using ToSic.Sxc.Engines;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Razor.Internal;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
[method: PrivateApi]
// ReSharper disable once UnusedMember.Global
public abstract class RazorTyped<TModel>()
    : OqtRazorBase<TModel>(CompatibilityLevels.CompatibilityLevel16, "Oqt.Rzr16"), IHasCodeLog, IRazor,
        ISetDynamicModel, IDynamicCode16, IHasCodeHelp
{
    #region ServiceKit

    /// <inheritdoc cref="IDynamicCode16.Kit"/>
    public ServiceKit16 Kit => _kit.Get(() => _CodeApiSvc.GetKit<ServiceKit16>());
    private readonly GetOnce<ServiceKit16> _kit = new();

    #endregion

    #region MyModel

    [PrivateApi("WIP v16.02")]
    public ITypedModel MyModel => CodeHelper.MyModel;

    #endregion

    #region New App, Settings, Resources

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => _CodeApiSvc.Link;

    /// <inheritdoc />
    public IAppTyped App => _CodeApiSvc.AppTyped;

    /// <inheritdoc cref="IDynamicCode16.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="IDynamicCode16.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion

    #region My... Stuff

    private TypedCode16Helper CodeHelper => RzrHlp.CodeHelper;

    public ITypedItem MyItem => CodeHelper.MyItem;

    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    public ITypedItem MyHeader => CodeHelper.MyHeader;

    public IBlockInstance MyData => _CodeApiSvc.Data;

    #endregion

    #region As Conversions

    /// <inheritdoc cref="IDynamicCode16.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => _CodeApiSvc.Cdf.AsItem(data, propsRequired: propsRequired ?? true, mock: mock);

    /// <inheritdoc cref="IDynamicCode16.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc.Cdf.AsItems(list, propsRequired: propsRequired ?? true);

    /// <inheritdoc cref="IDynamicCode16.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing)
        => _CodeApiSvc.Cdf.AsEntity(thing);

    /// <inheritdoc cref="IDynamicCode16.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc.Cdf.AsTyped(original, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => _CodeApiSvc.Cdf.AsTypedList(list, noParamOrder, propsRequired: propsRequired);

    /// <inheritdoc cref="IDynamicCode16.AsStack" />
    public ITypedStack AsStack(params object[] items)
        => _CodeApiSvc.Cdf.AsStack(items);

    /// <inheritdoc cref="IDynamicCode16.AsStack{T}" />
    public T AsStack<T>(params object[] items)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc.Cdf.AsStack<T>(items);

    #endregion


    /// <inheritdoc cref="IDynamicCode16.GetCode"/>
    public dynamic GetCode(string path, NoParamOrder noParamOrder = default, string className = default)
        => RzrHlp.GetCode(path, noParamOrder, className);

    #region MyContext & UniqueKey

    /// <inheritdoc cref="IDynamicCode16.MyContext" />
    public ICmsContext MyContext => _CodeApiSvc.CmsContext;

    /// <inheritdoc cref="IDynamicCode16.MyPage" />
    public ICmsPage MyPage => _CodeApiSvc.CmsContext.Page;

    /// <inheritdoc cref="IDynamicCode16.MyUser" />
    public ICmsUser MyUser => _CodeApiSvc.CmsContext.User;

    /// <inheritdoc cref="IDynamicCode16.MyView" />
    public ICmsView MyView => _CodeApiSvc.CmsContext.View;

    /// <inheritdoc cref="IDynamicCode16.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion

    #region Dev Tools & Dev Helpers

    [PrivateApi("Not yet ready")]
    public new IDevTools DevTools => CodeHelper.DevTools;

    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpForRazorTyped.Compile16;

    #endregion


    /// <summary>
    /// This is a tmp workaround to enable injecting the following properties in cshtml compiled with roslyn in Oqtane
    /// </summary>

    [RazorInject]
    public IModelExpressionProvider ModelExpressionProvider { get; set; } = null;

    [RazorInject]
    public IUrlHelper Url { get; set; } = null;

    [RazorInject]
    public IViewComponentHelper Component { get; set; } = null;

    [RazorInject]
    public IJsonHelper Json { get; set; } = null;

    [RazorInject]
    public IHtmlHelper<dynamic> Html { get; set; } = null;

    public PageContext PageContext { get; set; } = default!;

    #region As / AsList WIP v17

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder protector = default, bool mock = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc.Cdf.AsCustom<T>(source: source, protector: protector, mock: mock);

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _CodeApiSvc.Cdf.AsCustomList<T>(source: source, protector: protector, nullIfNull: nullIfNull);

    #endregion

    #region WIP v17

    /// <summary>
    /// EXPERIMENTAL support razor base class with typed model
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Introduced (beta) in v17.03
    /// </remarks>
    [PrivateApi("WIP, don't publish yet")]
    public new TModel Model => CodeHelper.GetModel<TModel>();

    /// <summary>
    /// WIP
    /// </summary>
    [PrivateApi("Experiment v17.02+")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected ICodeCustomizer Customize => _customize ??= _CodeApiSvc.GetService<ICodeCustomizer>(reuse: true);
    private ICodeCustomizer _customize;


    #endregion

}