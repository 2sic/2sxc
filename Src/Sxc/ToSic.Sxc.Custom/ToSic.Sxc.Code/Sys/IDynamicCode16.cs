using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

// TODO: SHOULD probably rename to ITypedCode16 or something

namespace ToSic.Sxc.Code.Sys;

/// <summary>
/// Standard interface for all TypedCode such as RazorPro or WebApiPro.
/// Provides typed APIs to access Settings, Resources and more.
/// </summary>
[PrivateApi("Shouldn't be visible, as the real API is 100% visible on RazorPro, CodePro etc.")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IDynamicCode16 : IGetCodePath, ICompatibilityLevel, IHasLog, IHasKit<ServiceKit16>
{
    #region Stuff basically inherited from v12/14

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    TService GetService<TService>() where TService : class;

    /// <inheritdoc cref="IDynamicCodeDocs.Link"/>
    ILinkService Link { get; }

    #endregion

    // #DropStrangeGetServiceWithTypeNameV20 - v20 removed again, not clear what this is for; wait & see, remove ca. 2025-Q3
    ///// <summary>
    ///// Advanced GetService which can do more than the standard GetService.
    ///// </summary>
    ///// <typeparam name="TService"></typeparam>
    ///// <param name="protector"></param>
    ///// <param name="typeName">Optional fully qualified type name to get the class based on a string identifier.</param>
    ///// <remarks>New in 17.06.01</remarks>
    ///// <returns></returns>
    //TService GetService<TService>(NoParamOrder protector = default, string? typeName = default) where TService : class;

    #region Kit

    ///// <inheritdoc cref="IDynamicCodeKit{TServiceKit}.Kit"/>
    //ServiceKit16 Kit { get; }

    #endregion

    #region Moving Properties

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




    #region App, Resources, Settings

    /// <inheritdoc cref="ITypedApi.App"/>
    IAppTyped App { get; }

    /// <inheritdoc cref="ITypedApi.AllResources"/>
    ITypedStack AllResources { get; }

    /// <inheritdoc cref="ITypedApi.AllSettings"/>
    ITypedStack AllSettings { get; }

    #endregion

    #region AsConversions

    /// <inheritdoc cref="ITypedApi.AsItem"/>
    ITypedItem AsItem(
        object data,
        NoParamOrder noParamOrder = default,
        bool? propsRequired = default,
        bool? mock = default
    );

    /// <inheritdoc cref="ITypedApi.AsItems"/>
    IEnumerable<ITypedItem> AsItems(
        object list,
        NoParamOrder noParamOrder = default,
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
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns></returns>
    ITyped AsTyped(
        object data,
        NoParamOrder noParamOrder = default,
        bool? propsRequired = default
    );

    /// <summary>
    /// Create a list
    /// </summary>
    /// <param name="list">List/Enumerable object containing a bunch of items to make typed</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="propsRequired">make the resulting object [strict](xref:NetCode.Conventions.PropertiesRequired), default `true`</param>
    /// <returns></returns>
    IEnumerable<ITyped> AsTypedList(
        object list,
        NoParamOrder noParamOrder = default,
        bool? propsRequired = default
    );

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
    public T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new();

    #endregion

    #region My... Stuff

    /// <summary>
    /// The main Item belonging to this Template/Module.
    /// This data is edited by the user directly on this specific module.
    /// In some cases it can also be a pre-set item configured in the View to be used if the user has not added any data himself.
    ///
    /// If this view can have a list of items (more than one) then this contains the first item.
    /// To get all the items, see <see cref="MyItems"/>
    /// </summary>
    ITypedItem MyItem {get; }

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

    #endregion

    /// <summary>
    /// Data passed to this Razor template by a caller.
    /// This is typical for Razor components which are re-used, and called from other Razor templates using `@Html.Partial("filename.cshtml", new { thing = 7 })`.
    /// </summary>
    ITypedRazorModel MyModel { get; }

    #region SharedCode

    /// <summary>
    /// Create an instance of a class in a `.cs` code file.
    /// Note that the class name in the file must match the file name, so `MyHelpers.cs` must have a `MyHelpers` class.
    /// </summary>
    /// <param name="path">The path, like `Helper.cs`, `./helper.cs`, `../../Helper.cs` or `/SomeFolderInApp/Helper.cs` (new 16.05)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="className">Optional class name, if it doesn't match the file name (new 16.03)</param>
    /// <returns>, </returns>
    /// <remarks>
    /// * Created in 16.02
    /// * `className` added in 16.03
    /// * Ability to give a path beginning with `/` as app-root in 16.05
    /// 
    /// In older code there was a similar `CreateInstance` method
    /// </remarks>
    dynamic? GetCode(string path, NoParamOrder noParamOrder = default, string? className = default);

    #endregion

    #region As Conversions

    /// <summary>
    /// Convert an Entity or TypedItem into a strongly typed object.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T">the target type</typeparam>
    /// <param name="source">the source object - an `IEntity` or `ITypedItem`</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="mock">if `true` will return a fake when `source` is `null` - otherwise a wrapper item with empty-contents</param>
    /// <returns></returns>
    /// <remarks>
    /// Released v17.05
    /// </remarks>
    T As<T>(object source, NoParamOrder protector = default, bool mock = default)
        where T : class, ICanWrapData;

    /// <summary>
    /// Convert a list of Entities or TypedItems into a strongly typed list.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T">the target type</typeparam>
    /// <param name="source">the source object - a List/Enumerable of `IEntity` or `ITypedItem`</param>
    /// <param name="protector">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="nullIfNull">if `true` will return null when `source` is `null` - otherwise a wrapper item with empty-contents</param>
    /// <returns></returns>
    /// <remarks>
    /// Release in v17.05
    /// </remarks>
    IEnumerable<T> AsList<T>(object source, NoParamOrder protector = default, bool nullIfNull = default)
        where T : class, ICanWrapData;

    #endregion
}