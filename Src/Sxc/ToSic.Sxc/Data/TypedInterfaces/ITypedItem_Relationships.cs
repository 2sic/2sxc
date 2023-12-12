using System.Collections.Generic;
using ToSic.Lib.Coding;

namespace ToSic.Sxc.Data;

public partial interface ITypedItem
{
    #region parents / children

    /// <inheritdoc cref="ITypedRelationships.Child"/>
    ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default);

    /// <inheritdoc cref="ITypedRelationships.Children"/>
    IEnumerable<ITypedItem> Children(string field = default, NoParamOrder noParamOrder = default, string type = default, bool? required = default);

    /// <inheritdoc cref="ITypedRelationships.Parent"/>
    ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default, string field = default);

    /// <inheritdoc cref="ITypedRelationships.Parents"/>
    IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder = default, string type = default, string field = default);

    #endregion

    /// <summary>
    /// True if this item version is published.
    /// This means that the item can exist as published, or published-with-draft, showing the published version.
    /// 
    /// _Note that by default, end-users only see the published version and don't see any draft version._
    /// </summary>
    /// <remarks>New in v17, see also <see cref="Publishing"/></remarks>
    bool IsPublished { get; }

    IPublishing Publishing { get; }
}