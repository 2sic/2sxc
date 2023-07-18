using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Run;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Standard interface for all TypedCode such as RazorPro or WebApiPro.
    /// Provides typed APIs to access Settings, Resources and more.
    /// </summary>
    [PrivateApi("Shouldn't be visible, as the real API is 100% visible on RazorPro, CodePro etc.")]
    public interface IDynamicCode16 : IGetCodePath, ICompatibilityLevel, IHasLog, IDynamicCodeKit<ServiceKit16>
    {
        #region Stuff basically inherited from v12/14

        /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
        TService GetService<TService>() where TService : class;

        /// <inheritdoc cref="IDynamicCode.Link"/>
        ILinkService Link { get; }

        #endregion

        #region Kit

        /// <inheritdoc cref="IDynamicCodeKit{TServiceKit}.Kit"/>
        ServiceKit16 Kit { get; }

        #endregion

        #region Moving Properties

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        ICmsContext MyContext { get; }

        
        /// <inheritdoc cref="ICmsContext.User" />
        ICmsUser MyUser { get; }

        /// <inheritdoc cref="ICmsContext.Page" />
        ICmsPage MyPage { get; }

        /// <inheritdoc cref="ICmsContext.View" />
        ICmsView MyView { get; }

        #endregion




        #region App, Resources, Settings

        IAppTyped App { get; }

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
        ITypedStack AllSettings{ get; }

        #endregion

        #region AsConversions

        /// <summary>
        /// Convert something to a <see cref="ITypedItem"/>.
        /// This works for all kinds of <see cref="IEntity"/>s,
        /// <see cref="IDynamicEntity"/>s as well as Lists/IEnumerables of those.
        /// 
        /// Will always return a single item.
        /// If a list is provided, it will return the first item in the list.
        /// If null was provided, it will return null.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <returns></returns>
        /// <remarks>New in v16.02</remarks>
        ITypedItem AsItem(
            object target,
            string noParamOrder = Eav.Parameters.Protector
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <returns></returns>
        /// <remarks>New in v16.01</remarks>
        IEnumerable<ITypedItem> AsItems(
            object list,
            string noParamOrder = Eav.Parameters.Protector
        );


        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        IEntity AsEntity(ICanBeEntity thing);

        /// <summary>
        /// Creates a typed object to read the original passed into this function.
        /// This is usually used to process objects which the compiler can't know, such as anonymous objects returned from helper code etc.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        /////
        ///// If you have an array of such objects, use <see cref="AsTypedList"/>.
        ITyped AsTyped(object original);

        ///// <summary>
        ///// TODO: WIP NAME NOT FINAL
        ///// </summary>
        ///// <param name="original"></param>
        ///// <returns></returns>
        //[PrivateApi]
        //IEnumerable<ITyped> AsTypedList(object original);

        /// <summary>
        /// Create a typed object which will provide all the properties of the things wrapped inside it.
        /// The priority is first-object first, so if multiple items have the property, the first in the list will be returned.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        ITypedStack AsStack(params object[] items);

        #endregion

        #region My... Stuff

        ITypedItem MyItem {get; }

        IEnumerable<ITypedItem> MyItems { get; }

        ITypedItem MyHeader { get; }

        IContextData MyData { get; }

        #endregion


        ITypedModel MyModel { get; }

        #region SharedCode

        /// <summary>
        /// Create an instance of a class in a `.cs` code file.
        /// Note that the class name in the file must match the file name, so `MyHelpers.cs` must have a `MyHelpers` class.
        /// </summary>
        /// <param name="path">The path, like `Helper.cs`, `./helper.cs` or `../../Helper.cs`</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="className">Optional class name, if it doesn't match the file name (new 16.03)</param>
        /// <returns>Created in 16.02, `className` added in 16.03</returns>
        dynamic GetCode(string path, string noParamOrder = Eav.Parameters.Protector, string className = default);

        #endregion

    }
}
