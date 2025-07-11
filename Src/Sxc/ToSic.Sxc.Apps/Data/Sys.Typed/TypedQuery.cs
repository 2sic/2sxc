using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Sxc.Apps.Sys.AppTyped;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys.Typed;

internal class TypedQuery(DataSourceBase.Dependencies services, LazySvc<QueryBuilder> queryBuilder)
    : Query(services, queryBuilder), ITypedQuery
{
    #region Kit Attachments

    internal TypedQuery Setup(ICodeDataFactory cdf)
    {
        CdfRequired = cdf;
        return this;
    }

    [field: AllowNull, MaybeNull]
    private ICodeDataFactory CdfRequired
    {
        get => field ?? throw new(nameof(CdfRequired) + " not set");
        set;
    }

    #endregion

    /// <inheritdoc />
    IEnumerable<T>? ITypedQuery.GetAll<T>(NoParamOrder protector, string? typeName, bool nullIfNotFound)
        => AppDataTyped.GetAllShared<T>(this, CdfRequired, typeName, nullIfNotFound, useDefaultIfNameNotSetAndNotFound: true);

    /// <inheritdoc />
    T? ITypedQuery.GetOne<T>(int id, NoParamOrder protector, bool skipTypeCheck)
        where T : class
        => CdfRequired.GetOne<T>(() => List.One(id), id, skipTypeCheck);

    /// <inheritdoc />
    T? ITypedQuery.GetOne<T>(Guid id, NoParamOrder protector, bool skipTypeCheck)
        where T : class
        => CdfRequired.GetOne<T>(() => List.One(id), id, skipTypeCheck);

}
