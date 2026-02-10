using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys.Typed;

internal class TypedQuery(DataSourceBase.Dependencies services, LazySvc<QueryFactory> queryBuilder, LazySvc<QueryDefinitionFactory> queryDefBuilder)
    : Query(services, queryBuilder, queryDefBuilder), ITypedQuery
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
    IEnumerable<T>? ITypedQuery.GetAll<T>(NoParamOrder npo, string? typeName, bool nullIfNotFound)
        => ToTypedHelper.GetAllShared<T>(typeName, nullIfNotFound, useDefaultIfNameNotSetAndNotFound: true);

    /// <inheritdoc />
    T? ITypedQuery.GetOne<T>(int id, NoParamOrder npo, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(id, npo, skipTypeCheck);

    /// <inheritdoc />
    T? ITypedQuery.GetOne<T>(Guid guid, NoParamOrder npo, bool skipTypeCheck)
        where T : class
        => ToTypedHelper.GetOne<T>(guid, npo, skipTypeCheck);

}
