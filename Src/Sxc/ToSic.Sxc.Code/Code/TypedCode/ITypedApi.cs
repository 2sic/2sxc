using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Code;

/// <summary>
/// This API describes objects with a TypedApi.
/// </summary>
/// <remarks>
/// This is usually only relevant when using the `ITypedApiService` which will get you a standalone object with this API.
/// </remarks>
[WorkInProgressApi("Still WIP v20")]
public interface ITypedApi
{
    #region App, Resources, Settings

    /// <summary>
    /// The current App object (with strictly typed Settings/Resources).
    /// Use it to access App properties such as `Path` or any data in the App.
    /// </summary>
    public IAppTyped App { get; }

    /// <summary>
    /// Stack of all Resources in the System, merging Resources of View, App, Site, Global etc.
    /// Will retrieve values by priority, with View-Resources being top priority and Preset-Resources being the lowest.
    ///
    /// > [!TIP]
    /// > If you know that Resources come from the App, you should prefer `App.Resources` instead.
    /// > That is faster and helps people reading your code figure out where to change a value.
    /// </summary>
    ITypedStack AllResources { get; }

    /// <summary>
    /// Stack of all Settings in the System, merging Settings of View, App, Site, Global etc.
    /// Will retrieve values by priority, with View-Settings being top priority and Preset-Settings being the lowest.
    ///
    /// > [!TIP]
    /// > If you know that Settings come from the App, you should prefer `App.Settings` instead.
    /// > That is faster and helps people reading your code figure out where to change a value.
    /// </summary>
    ITypedStack AllSettings { get; }

    /// <summary>
    /// The main Item belonging to this Template/Module.
    /// This data is edited by the user directly on this specific module.
    /// In some cases it can also be a pre-set item configured in the View to be used if the user has not added any data himself.
    ///
    /// If this view can have a list of items (more than one) then this contains the first item.
    /// To get all the items, see <see cref="MyItems"/>
    /// </summary>
    ITypedItem MyItem { get; }

    /// <summary>
    /// List of all Items belonging to this Template/Module.
    /// This data is edited by the user directly on this specific module.
    /// In some cases it can also be a pre-set item configured in the View to be used if the user has not added any data himself.
    ///
    /// If this view is configured to only have one item, then this list will only contain one item.
    /// Otherwise, it will have as many items as the editor added.
    /// </summary>
    IEnumerable<ITypedItem> MyItems { get; }

    /// <summary>
    /// The Header-Item belonging to this Template/Module.
    /// This data is edited by the user directly on this specific module.
    /// In some cases it can also be a pre-set item configured in the View to be used if the user has not added any data himself.
    /// </summary>
    ITypedItem MyHeader { get; }

    /// <summary>
    /// All the data which the current Template received, based on the View configuration.
    /// There are a few common scenarios:
    ///
    /// 1. If it's a simple view, then this will just contain streams with the main Item(s) and Header
    /// 1. If the view expects no data, it will just contain a `Default` stream containing no items
    /// 1. If the view has a Query behind it, then MyData will have all the streams provided by the Query
    /// </summary>
    IDataSource MyData { get; }

    /// <inheritdoc cref="IHasKit{TServiceKit}"/>
    ServiceKit16 Kit { get; }

    /// <inheritdoc cref="IDynamicCodeDocs.Link"/>
    ILinkService Link { get; }

    #endregion

    #region MyContext / UniqueKey

    /// <inheritdoc cref="IDynamicCodeDocs.CmsContext" />
    ICmsContext MyContext { get; }

    /// <inheritdoc cref="ICmsContext.Page" />
    ICmsPage MyPage { get; }

    /// <inheritdoc cref="ICmsContext.User" />
    ICmsUser MyUser { get; }

    /// <inheritdoc cref="ICmsContext.View" />
    ICmsView MyView { get; }

    /// <inheritdoc cref="IKeyService.UniqueKey"/>
    string UniqueKey { get; }

    #endregion

    /// <summary>
    /// Convert something to a <see cref="ITypedItem"/>.
    /// This works for all kinds of <see cref="IEntity"/>s,
    /// <see cref="IDynamicEntity"/>s as well as Lists/Enumerables of those.
    /// 
    /// Will always return a single item.
    /// If a list is provided, it will return the first item in the list.
    /// If null was provided, it will return null.
    /// </summary>
    /// <param name="data">An original object which can be converted to a TypedItem, such as a <see cref="IEntity"/> .</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns></returns>
    /// <remarks>
    /// New in v16.02
    /// Parameter `mock` removed in v21 (breaking, but probably never used in the wild); use `Convert.ToMock()` instead
    /// </remarks>
    ITypedItem? AsItem(
        object data,
        NoParamOrder npo = default,
        bool? propsRequired = default
    );

    /// <summary>
    /// Convert an object containing a list of Entities or similar to a list of <see cref="ITypedItem"/>s.
    /// </summary>
    /// <param name="list">The original list which is usually a list of <see cref="IEntity"/> objects.</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns></returns>
    /// <remarks>New in v16.01</remarks>
    IEnumerable<ITypedItem> AsItems(
        object list,
        NoParamOrder npo = default,
        bool? propsRequired = default
    );


    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    IEntity AsEntity(ICanBeEntity thing);

    /// <summary>
    /// Creates a typed object to read the original passed into this function.
    /// This is usually used to process objects which the compiler can't know, such as anonymous objects returned from helper code etc.
    /// 
    /// If you have an array of such objects, use <see cref="AsTypedList"/>.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns></returns>
    ITyped AsTyped(object data, NoParamOrder npo = default, bool? propsRequired = default);

    /// <summary>
    /// Create a list
    /// </summary>
    /// <param name="list">List/Enumerable object containing a bunch of items to make typed</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns></returns>
    IEnumerable<ITyped> AsTypedList(object list, NoParamOrder npo = default, bool? propsRequired = default);

    /// <summary>
    /// Create a typed object which will provide all the properties of the things wrapped inside it.
    /// The priority is first-object first, so if multiple items have the property, the first in the list will be returned.
    /// </summary>
    /// <param name="items">objects to stack together</param>
    /// <returns></returns>
    ITypedStack AsStack(params object[] items);

    /// <summary>
    /// Create a custom-typed object which will provide all the properties of the things wrapped inside it.
    /// The priority is first-object first, so if multiple items have the property, the first in the list will be returned.
    /// </summary>
    /// <param name="items">objects to stack together</param>
    /// <returns>Item of the custom type</returns>
    /// <remarks>New in 17.07</remarks>
    T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new();

    /// <summary>
    /// Convert an Entity or TypedItem into a strongly typed object.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T">the target type</typeparam>
    /// <param name="source">the source object - an `IEntity` or `ITypedItem`</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <returns></returns>
    /// <remarks>
    /// Released v17.05
    /// Parameter `mock` removed in v21 (breaking, but probably never used in the wild); use `Convert.ToMock()` instead
    /// </remarks>
    T As<T>(object source, NoParamOrder npo = default)
        where T : class, ICanWrapData;

    /// <summary>
    /// Convert a list of Entities or TypedItems into a strongly typed list.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T">the target type</typeparam>
    /// <param name="source">the source object - a List/Enumerable of `IEntity` or `ITypedItem`</param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="nullIfNull">if `true` will return null when `source` is `null` - otherwise a wrapper item with empty-contents</param>
    /// <returns></returns>
    /// <remarks>
    /// Release in v17.05
    /// </remarks>
    IEnumerable<T> AsList<T>(object source, NoParamOrder npo = default, bool nullIfNull = default)
        where T : class, ICanWrapData;
}
