using System.Collections.Generic;
using ToSic.Lib.Coding;
using ToSic.Sxc.Data.Internal.Docs;

namespace ToSic.Sxc.Data;

partial interface ITypedItem
{
    #region parents / children

    /// <inheritdoc cref="ITypITypedRelationshipsDocsld"/>
    ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default);

    /// <inheritdoc cref="ITypedRelationshipsDocs.Children"/>
    IEnumerable<ITypedItem> Children(string field = default, NoParamOrder noParamOrder = default, string type = default, bool? required = default);

    /// <inheritdoc cref="ITypedRelationshipsDocs.Parent"/>
    ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default, string field = default);

    /// <inheritdoc cref="ITypedRelationshipsDocs.Parents"/>
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