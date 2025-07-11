using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys.Typed;

internal class TypedQuery(DataSourceBase.Dependencies services, LazySvc<QueryBuilder> queryBuilder)
    : Query(services, queryBuilder), ITypedQuery
{
    #region Cdf Attachments and setup ToTypedHelper

    internal TypedQuery Setup(ICodeDataFactory cdfConnected)
    {
        ToTypedHelper = new(cdfConnected, this, Log);
        return this;
    }

    [field: AllowNull, MaybeNull]
    private DataSourceToTypedHelper ToTypedHelper { get => field ?? throw new($"{nameof(ToTypedHelper)} not set"); set; }

    #endregion

    /// <inheritdoc />
    IEnumerable<T>? ITypedQuery.GetAll<T>(NoParamOrder protector, string? typeName, bool nullIfNotFound)
        => ToTypedHelper.GetAllShared<T>(typeName, nullIfNotFound, useDefaultIfNameNotSetAndNotFound: true);

    /// <inheritdoc />
    T? ITypedQuery.GetOne<T>(int id, NoParamOrder protector, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(id, protector, skipTypeCheck);

    /// <inheritdoc />
    T? ITypedQuery.GetOne<T>(Guid id, NoParamOrder protector, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(id, protector, skipTypeCheck);

}
