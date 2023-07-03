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
    [WorkInProgressApi("WIP 16.02")]
    public interface IDynamicCode16 : ICreateInstance, ICompatibilityLevel, IHasLog
    {
        #region Stuff basically inherited from v12/14

        /// <inheritdoc cref="IDynamicCode.GetService{TService}"/>
        TService GetService<TService>();

        ///// <inheritdoc cref="IDynamicCode14{TModel,TServiceKit}.AsAdam"/>
        //IFolder AsAdam(ICanBeEntity item, string fieldName);

        /// <inheritdoc cref="IDynamicCode.Link"/>
        ILinkService Link { get; }

        ///// <inheritdoc cref="IDynamicCode14{TModel,TServiceKit}.Edit"/>
        //IEditService Edit { get; }

        ///// <inheritdoc cref="IDynamicCode14{TModel,TServiceKit}.CmsContext"/>
        //ICmsContext CmsContext { get; }

        #endregion

        #region Moving Properties

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        ICmsContext MyContext { get; }

        
        /// <inheritdoc cref="ICmsContext.User" />
        ICmsUser MyUser { get; }

        /// <inheritdoc cref="ICmsContext.Page" />
        ICmsPage MyPage { get; }

        ICmsView MyView { get; }

        #endregion

        #region AsEntity

        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        IEntity AsEntity(ICanBeEntity thing);

        #endregion



        #region Stuff Added in v16

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

        IMyData MyData { get; }

        #endregion


        ITypedModel MyModel { get; }

        #region SharedCode

        //SharedCode Code(string path);

        #endregion

    }
}
