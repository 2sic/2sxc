using System.Collections.Generic;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedItem
    {
        #region parents / children

        /// <inheritdoc cref="ITypedRelationships.Child"/>
        ITypedItem Child(string name, string noParamOrder = Protector);

        /// <inheritdoc cref="ITypedRelationships.Children"/>
        IEnumerable<ITypedItem> Children(string field = default, string noParamOrder = Protector, string type = default);

        /// <inheritdoc cref="ITypedRelationships.Parents"/>
        IEnumerable<ITypedItem> Parents(string type = default, string noParamOrder = Protector, string field = default);

        #endregion 
    }
}
