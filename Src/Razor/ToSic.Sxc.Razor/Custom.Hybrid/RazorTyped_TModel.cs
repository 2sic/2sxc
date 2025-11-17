using System.Diagnostics.CodeAnalysis;
using Custom.Razor.Sys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;
using ToSic.Sys.Code.Help;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

// ReSharper disable once UnusedMember.Global
public abstract class RazorTyped<TModel>()
    : OqtRazorBase<TModel>(CompatibilityLevels.CompatibilityLevel16, "Oqt.Rzr16"), IHasCodeLog, IRazor,
        ISetDynamicModel, ITypedCode16, IHasCodeHelp
{
    #region ServiceKit

    [field: AllowNull, MaybeNull]
    internal ICodeTypedApiHelper CodeApi => field ??= RzrHlp.ExCtxRoot.GetTypedApi();

    /// <inheritdoc cref="IHasKit{TServiceKit}.Kit"/>
    [field: AllowNull, MaybeNull]
    public ServiceKit16 Kit => field ??= CodeApi.ServiceKit16;

    #endregion

    #region MyModel

    // [PrivateApi("WIP v16.02")]
    public ITypedRazorModel MyModel => CodeHelper.MyModel;

    #endregion

    #region New App, Settings, Resources

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    public ILinkService Link => CodeApi.Link;

    /// <inheritdoc />
    public IAppTyped App => CodeApi.AppTyped;

    /// <inheritdoc cref="ITypedApi.AllResources" />
    public ITypedStack AllResources => CodeHelper.AllResources;

    /// <inheritdoc cref="ITypedApi.AllSettings" />
    public ITypedStack AllSettings => CodeHelper.AllSettings;

    #endregion

    #region My... Stuff

    private TypedCode16Helper CodeHelper => RzrHlp.CodeHelper;

    public ITypedItem MyItem => CodeHelper.MyItem;

    public IEnumerable<ITypedItem> MyItems => CodeHelper.MyItems;

    public ITypedItem MyHeader => CodeHelper.MyHeader;

    public IDataSource MyData => CodeApi.Data;

    #endregion

    #region As Conversions

    /// <inheritdoc cref="ITypedApi.AsItem" />
    public ITypedItem AsItem(object data, NoParamOrder noParamOrder = default, bool? propsRequired = default, bool? mock = default)
        => CodeApi.Cdf.AsItem(data, new() { ItemIsStrict = propsRequired ?? true, UseMock = mock == true })!;

    /// <inheritdoc cref="ITypedApi.AsItems" />
    public IEnumerable<ITypedItem> AsItems(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsItems(list, new() { ItemIsStrict = propsRequired ?? true });

    /// <inheritdoc cref="ITypedApi.AsEntity" />
    public IEntity AsEntity(ICanBeEntity thing)
        => CodeApi.Cdf.AsEntity(thing);

    /// <inheritdoc cref="ITypedApi.AsTyped" />
    public ITyped AsTyped(object original, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTyped(original, new() { EntryPropIsRequired = false, ItemIsStrict = propsRequired ?? true })!;

    /// <inheritdoc cref="ITypedApi.AsTypedList" />
    public IEnumerable<ITyped> AsTypedList(object list, NoParamOrder noParamOrder = default, bool? propsRequired = default)
        => CodeApi.Cdf.AsTypedList(list, new() { EntryPropIsRequired = false, ItemIsStrict = propsRequired ?? true })!;

    /// <inheritdoc cref="ITypedApi.AsStack" />
    public ITypedStack AsStack(params object[] items)
        => CodeApi.Cdf.AsStack(items);

    /// <inheritdoc cref="ITypedApi.AsStack{T}" />
    public T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new()
        => CodeApi.Cdf.AsStack<T>(items);

    #endregion


    /// <inheritdoc cref="ITypedCode16.GetCode"/>
    public dynamic? GetCode(string path, NoParamOrder noParamOrder = default, string? className = default)
        => RzrHlp.GetCode(path, noParamOrder, className);

    #region MyContext & UniqueKey

    /// <inheritdoc cref="ITypedApi.MyContext" />
    public ICmsContext MyContext => CodeApi.CmsContext;

    /// <inheritdoc cref="ITypedApi.MyPage" />
    public ICmsPage MyPage => CodeApi.CmsContext.Page;

    /// <inheritdoc cref="ITypedApi.MyUser" />
    public ICmsUser MyUser => CodeApi.CmsContext.User;

    /// <inheritdoc cref="ITypedApi.MyView" />
    public ICmsView MyView => CodeApi.CmsContext.View;

    /// <inheritdoc cref="ITypedApi.UniqueKey" />
    public string UniqueKey => Kit.Key.UniqueKey;

    #endregion

    #region Dev Tools & Dev Helpers

    // TODO: [PrivateApi("Not yet ready")] - Attribute not found in current dependencies, please review if it should be replaced or removed.
    public new IDevTools DevTools => CodeHelper.DevTools;

    // TODO: [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpDbRazor.Compile16; - Attribute not found in current dependencies, please review if it should be replaced or removed.
    List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpDbRazor.Compile16;

    #endregion


    /// <summary>
    /// This is a tmp workaround to enable injecting the following properties in cshtml compiled with roslyn in Oqtane
    /// </summary>

    [RazorInject]
    public IModelExpressionProvider ModelExpressionProvider { get; set; } = null!;

    [RazorInject]
    public IUrlHelper Url { get; set; } = null!;

    [RazorInject]
    public IViewComponentHelper Component { get; set; } = null!;

    [RazorInject]
    public IJsonHelper Json { get; set; } = null!;

    [RazorInject]
    public IHtmlHelper<dynamic> Html { get; set; } = null!;

    public PageContext PageContext { get; set; } = null!;

    #region As / AsList WIP v17

    /// <inheritdoc />
    public T As<T>(object source, NoParamOrder protector = default, bool mock = false)
        where T : class, ICanWrapData
        => CodeApi.Cdf.AsCustom<T>(source: source, protector: protector, mock: mock)!;

    /// <inheritdoc />
    public IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ICanWrapData
        => CodeApi.Cdf.AsCustomList<T>(source: source, protector: protector, nullIfNull: nullIfNull);

    #endregion

    #region WIP v17

    /// <summary>
    /// Typed Model of a Razor with typed model
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// * Introduced in v17.03 (beta)
    /// * Stable since v18.00
    /// </remarks>
    // TODO: [PrivateApi("WIP, don't publish yet")] - Attribute not found in current dependencies, please review if it should be replaced or removed.
    public new TModel Model => CodeHelper.GetModel<TModel>();

    /// <inheritdoc cref="CodeTyped.Customize"/>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    protected ICodeCustomizer Customize => field ??= CodeApi.GetService<ICodeCustomizer>(reuse: true);

    #endregion

}