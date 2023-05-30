using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    public partial interface IDynamicCode14<out TModel, out TServiceKit>
    {
        #region Stuff Added in v16

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
        /// <remarks>New in v16.01</remarks>
        ITypedItem AsTyped(
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
        IEnumerable<ITypedItem> AsTypedList(
            object list,
            string noParamOrder = Eav.Parameters.Protector
        );


        #endregion
    }
}
