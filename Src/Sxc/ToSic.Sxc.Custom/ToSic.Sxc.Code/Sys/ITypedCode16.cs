using ToSic.Eav.DataSource;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
// ReSharper disable MethodOverloadWithOptionalParameter

namespace ToSic.Sxc.Code.Sys;

/// <summary>
/// Standard interface for all TypedCode such as RazorPro or WebApiPro.
/// Provides typed APIs to access Settings, Resources and more.
/// </summary>
[PrivateApi("Shouldn't be visible, as the real API is 100% visible on RazorPro, CodePro etc.")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ITypedCode16 : IGetCodePath, ICompatibilityLevel, IHasLog, IHasKit<ServiceKit16>
{
    #region Stuff basically inherited from v12/14

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    TService GetService<TService>() where TService : class;

    /// <inheritdoc cref="IDynamicCodeDocs.Link"/>
    ILinkService Link { get; }

    #endregion

    /// <summary>
    /// Advanced GetService which can do more than the standard GetService.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="typeName">Optional fully qualified type name to get the class based on a string identifier.</param>
    /// <remarks>New in 17.06.01</remarks>
    /// <returns></returns>
    /// <remarks>
    /// This is commonly used for scenarios where the editor
    /// might select a file from the AppCode to be used for something specific,
    /// and then the code needs to run this service.
    /// 
    /// For example in Mobius Forms, News etc. where a developer might create a new mail template and the editor can select it from the files in the folder.
    /// </remarks>
    TService GetService<TService>(NoParamOrder npo = default, string? typeName = default) where TService : class;

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
        NoParamOrder npo = default,
        bool? propsRequired = default
    );

    /// <inheritdoc cref="ITypedApi.AsItems"/>
    IEnumerable<ITypedItem> AsItems(
        object list,
        NoParamOrder npo = default,
        bool? propsRequired = default
    );


    /// <inheritdoc cref="IDynamicCodeDocs.AsEntity" />
    IEntity AsEntity(ICanBeEntity thing);

    /// <inheritdoc cref="ITypedApi.AsTyped"/>
    ITyped AsTyped(
        object data,
        NoParamOrder npo = default,
        bool? propsRequired = default
    );

    /// <inheritdoc cref="ITypedApi.AsTypedList"/>
    IEnumerable<ITyped> AsTypedList(
        object list,
        NoParamOrder npo = default,
        bool? propsRequired = default
    );

    /// <inheritdoc cref="ITypedApi.AsStack"/>
    ITypedStack AsStack(params object[] items);

    /// <inheritdoc cref="ITypedApi.AsStack{T}"/>
    public T AsStack<T>(params object[] items)
        where T : class, ICanWrapData, new();

    #endregion

    #region My... Stuff

    /// <inheritdoc cref="ITypedApi.MyItem"/>
    ITypedItem MyItem {get; }

    /// <inheritdoc cref="ITypedApi.MyItems"/>
    IEnumerable<ITypedItem> MyItems { get; }

    /// <inheritdoc cref="ITypedApi.MyHeader"/>
    ITypedItem MyHeader { get; }

    /// <inheritdoc cref="ITypedApi.MyData"/>
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
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="className">Optional class name, if it doesn't match the file name (new 16.03)</param>
    /// <returns>, </returns>
    /// <remarks>
    /// * Created in 16.02
    /// * `className` added in 16.03
    /// * Ability to give a path beginning with `/` as app-root in 16.05
    /// 
    /// In older code there was a similar `CreateInstance` method
    /// </remarks>
    dynamic? GetCode(string path, NoParamOrder npo = default, string? className = default);

    #endregion

    #region As Conversions

    /// <inheritdoc cref="ITypedApi.As{T}"/>
    T As<T>(object source, NoParamOrder npo = default)
        where T : class, ICanWrapData;

    /// <inheritdoc cref="ITypedApi.AsList{T}"/>
    IEnumerable<T> AsList<T>(object source, NoParamOrder npo = default, bool nullIfNull = default)
        where T : class, ICanWrapData;

    #endregion
}