using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// Standard interface for all TypedCode such as Razor16 or WebApi16.
    /// Provides typed APIs to access Settings, Resources and more.
    /// </summary>
    [WorkInProgressApi("WIP 16.02")]
    public interface IDynamicCode16
    {
        #region Stuff Added in v16

        IAppTyped App { get; }

        // TODO: remove once all apps are migrated
        ITypedStack Settings { get; }

        // TODO: remove once all apps are migrated
        ITypedStack Resources { get; }

        ITypedStack ResourcesStack { get; }

        ITypedStack SettingsStack { get; }

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
        ITypedRead Merge(params object[] items);

        #endregion
    }
}
