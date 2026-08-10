#if NETFRAMEWORK
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeApiService;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeApiServiceObsolete(IExecutionContext dynCode)
{
    [PrivateApi("obsolete")]
    [Obsolete("you should use the CreateSource<T> instead. Deprecated ca. v4 (but not sure), changed to error in v15.")]
    public IDataSource CreateSource(string typeName = "", IDataSource? links = null, ILookUpEngine? configuration = null)
    {
        // 2023-03-12 2dm
        // Completely rewrote this, because I got rid of some old APIs in v15 on the DataFactory
        // This has never been tested but probably works, but we won't invest time to be certain.

        var dataSources = ((ExecutionContext)dynCode).DataSources;

        try
        {
            // try to find with assembly name, or otherwise with GlobalName / previous names
            var app = dynCode.GetApp();
            var type = dataSources.Catalog.Value.FindDataSourceInfo(typeName, app.AppId)?.Type;
            configuration ??= dataSources.LookUpEngine;
            var cnf2Wip = new DataSourceOptions
            {
                AppIdentityOrReader = null, // #WipAppIdentityOrReader must become not null
                LookUp = configuration,
                Attach = links,
            };
            if (links != null)
                return dataSources.DataSources.Value.Create(type: type!, options: cnf2Wip);

            var initialSource = dataSources.DataSources.Value.CreateDefault(new DataSourceOptions
            {
                AppIdentityOrReader = app,
                LookUp = dataSources.LookUpEngine,
            });
            return typeName != ""
                ? dataSources.DataSources.Value.Create(type: type!, options: cnf2Wip with { Attach = initialSource })
                : initialSource;
        }
        catch (Exception ex)
        {
            const string errMessage = $"The razor code is calling a very old method {nameof(CreateSource)}." +
                                      $" In this version, you used the type name as a string {nameof(CreateSource)}(string typeName, ...)." +
                                      $" This has been deprecated since ca. v4 and has been removed now. " +
                                      $" Please use the newer {nameof(CreateSource)}<Type>(...) overload.";

            throw new(errMessage, ex);
        }
    }


}

#endif