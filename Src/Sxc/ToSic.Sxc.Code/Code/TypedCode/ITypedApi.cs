using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code;

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

    IDataSource Data { get; }

    /// <inheritdoc />
    ITypedItem MyItem { get; }

    /// <inheritdoc />
    IEnumerable<ITypedItem> MyItems { get; }

    /// <inheritdoc />
    ITypedItem MyHeader { get; }

    /// <inheritdoc />
    IDataSource MyData { get; }

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
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <param name="mock">Specify that the data is fake/mock data, which should pretend to be an Item. Default is `false`</param>
    /// <returns></returns>
    /// <remarks>New in v16.02</remarks>
    ITypedItem? AsItem(
        object data,
        NoParamOrder noParamOrder = default,
        bool? propsRequired = default,
        bool? mock = default
    );

    /// <summary>
    /// Convert an object containing a list of Entities or similar to a list of <see cref="ITypedItem"/>s.
    /// </summary>
    /// <param name="list">The original list which is usually a list of <see cref="IEntity"/> objects.</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns></returns>
    /// <remarks>New in v16.01</remarks>
    IEnumerable<ITypedItem> AsItems(
        object list,
        NoParamOrder noParamOrder = default,
        bool? propsRequired = default
    );


    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    IEntity AsEntity(ICanBeEntity thing);
}
