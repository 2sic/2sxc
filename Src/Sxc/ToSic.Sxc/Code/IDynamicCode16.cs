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
    /// Standard interface for all TypedCode such as Razor16 or WebApi16.
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

        /// <inheritdoc cref="IDynamicCode14{TModel,TServiceKit}.CmsContext"/>
        ICmsContext MyContext { get; }



        #endregion

        #region AsEntity

        /// <inheritdoc cref="IDynamicCode.AsEntity" />
        IEntity AsEntity(object dynamicEntity);

        #endregion



        #region from V16 WIP

        /// <summary>
        /// Take a json and provide it as a dynamic object to the code
        /// </summary>
        /// <remarks>Added in 2sxc 10.22.00</remarks>
        /// <param name="json">the original json string</param>
        /// <param name="fallback">
        /// Alternate string to use, if the original json can't parse.
        /// Can also be null or the word "error" if you would prefer an error to be thrown.</param>
        /// <returns>A dynamic object representing the original json.
        /// If it can't be parsed, it will parse the fallback, which by default is an empty empty dynamic object.
        /// If you provide null for the fallback, then you will get null back.
        /// </returns>
        ITypedRead Read(string json, string fallback = default);

        #endregion

        #region Stuff Added in v16

        IAppTyped App { get; }

        //// TODO: remove once all apps are migrated
        //ITypedStack Settings { get; }

        //// TODO: remove once all apps are migrated
        //ITypedStack Resources { get; }

        ITypedStack ResourcesStack { get; }

        ITypedStack SettingsStack { get; }

        ITypedStack SysResources { get; }

        ITypedStack SysSettings{ get; }

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
        ITypedStack Merge(params object[] items);

        ITypedStack AsStack(params object[] items);

        #endregion

        #region My... Stuff

        ITypedItem MyItem {get; }

        IEnumerable<ITypedItem> MyItems { get; }

        ITypedItem MyHeader { get; }

        IMyData MyData { get; }

        #endregion


        ITypedModel MyModel { get; }

    }
}
