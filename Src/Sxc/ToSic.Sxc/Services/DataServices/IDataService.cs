using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    [PrivateApi("not yet ready / public")]
    public interface IDataService
    {
        #region CreateDataSource - new in v15 - make sure it's copied in identical form to IDynamicCode, ...

        /// <summary>
        /// Create a DataSource object using it's type.
        /// This is the new, preferred way to get DataSources in v15.06+.
        /// </summary>
        /// <typeparam name="T">The type of DataSource, usually from [](xref:ToSic.Eav.DataSources) or [](xref:ToSic.Sxc.DataSources)</typeparam>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="attach">Link to one or more other DataSources / streams to attach upon creation.</param>
        /// <param name="options">Options how to build/construct the DataSource - especially parameters to set. See TODO: </param>
        /// <remarks>WIP v15.06 BETA</remarks>
        /// <returns></returns>
        [PrivateApi]
        T GetSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource;

        /// <summary>
        /// Create a DataSource object using it's name.
        /// This is only meant for dynamically compiled DataSources which are part of the current App - a new feature in v15.10+.
        /// For any other DataSources, use the overload which specifies the type. 
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="name">The name of the DataSource type, which matches the file name and class in the `/DataSources/` folder.</param>
        /// <param name="attach">Link to one or more other DataSources / streams to attach upon creation.</param>
        /// <param name="options">Options how to build/construct the DataSource - especially parameters to set. See TODO: </param>
        /// <remarks>WIP v15.06 BETA</remarks>
        /// <returns></returns>
        [PrivateApi]
        IDataSource GetSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = default, object options = default);

        #endregion
    }
}
