using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// WIP v15.07 - new services to create DataSources in Razor as well as external (skin) use.
    /// </summary>
    [WorkInProgressApi("not yet ready / public")]
    public interface IDataService
    {
        #region CreateDataSource - new in v15 - make sure it's copied in identical form to IDynamicCode, ...

        /// <summary>
        /// Spawn a new <see cref="IDataService"/> with specific configuration.
        /// Uses the [Spawn New convention](xref:NetCode.Conventions.SpawnNew).
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="appIdentity"></param>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        IDataService New(string noParamOrder = Protector,
            IAppIdentity appIdentity = default,
            int zoneId = default,
            int appId = default);

        /// <summary>
        /// Get the App DataSource containing the App Data.
        /// The `Default` stream of this source has the data the current user is allowed to see.
        /// So public users won't get draft data.
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="parameters">Parameters to use - as anonymous object like `new { Count = 7, Filter = 3 }`</param>
        /// <param name="options"></param>
        /// <returns></returns>
        IDataSource GetAppSource(string noParamOrder = Protector, object parameters = default, object options = default);

        /// <summary>
        /// Create a DataSource object using it's type.
        /// This is the new, preferred way to get DataSources in v15.06+.
        /// </summary>
        /// <typeparam name="T">The type of DataSource, usually from [](xref:ToSic.Eav.DataSources) or [](xref:ToSic.Sxc.DataSources)</typeparam>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="attach">Link to one or more other DataSources / streams to attach upon creation.</param>
        /// <param name="parameters">Parameters to use - as anonymous object like `new { Count = 7, Filter = 3 }`</param>
        /// <param name="options">Options how to build/construct the DataSource - especially parameters to set. See TODO: </param>
        /// <remarks>WIP v15.07 BETA</remarks>
        /// <returns></returns>
        T GetSource<T>(string noParamOrder = Protector,
            IDataSourceLinkable attach = default,
            object parameters = default,
            object options = default
        ) where T : IDataSource;

        /// <summary>
        /// Create a DataSource object using it's name.
        /// This is only meant for dynamically compiled DataSources which are part of the current App - a new feature in v15.10+.
        /// For any other DataSources, use the overload which specifies the type. 
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="name">The name of the DataSource type, which matches the file name and class in the `/DataSources/` folder.</param>
        /// <param name="attach">Link to one or more other DataSources / streams to attach upon creation.</param>
        /// <param name="parameters">Parameters to use - as anonymous object like `new { Count = 7, Filter = 3 }`</param>
        /// <param name="options">Options how to build/construct the DataSource - especially parameters to set. See TODO: </param>
        /// <remarks>WIP v15.07</remarks>
        /// <returns></returns>
        IDataSource GetSource(string noParamOrder = Protector,
            string name = default,
            IDataSourceLinkable attach = default,
            object parameters = default,
            object options = default
        );

        /// <summary>
        /// Get a Query from the current App.
        /// </summary>
        /// <param name="name">Name of the query</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="attach">Attach in-stream to the query (not yet implemented)</param>
        /// <param name="parameters">Parameters to use - as anonymous object like `new { Count = 7, Filter = 3 }`</param>
        /// <returns></returns>
        /// <remarks>New 16.00.01 (should have been in 16.00 but was forgotten)</remarks>
        IDataSource GetQuery(
            string name = default,
            string noParamOrder = Protector,
            IDataSourceLinkable attach = default,
            object parameters = default
        );

        #endregion
    }
}
