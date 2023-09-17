using System.Collections.Generic;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedItem
    {
        #region parents / children

        /// <inheritdoc cref="ITypedRelationships.Child"/>
        ITypedItem Child(string name, string noParamOrder = Protector, bool? required = default);

        /// <inheritdoc cref="ITypedRelationships.Children"/>
        IEnumerable<ITypedItem> Children(string field = default, string noParamOrder = Protector, string type = default, bool? required = default);

        [PrivateApi]
        ITypedItem Parent();

        /// <inheritdoc cref="ITypedRelationships.Parents"/>
        IEnumerable<ITypedItem> Parents(string noParamOrder = Protector, string type = default, string field = default);

        #endregion 
    }
}
