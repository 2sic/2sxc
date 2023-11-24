using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using static ToSic.Eav.Parameters;

// ReSharper disable UnusedMemberInSuper.Global

namespace ToSic.Sxc.Apps;

/// <summary>
/// An app-object as is available in a razor template or WebApi
/// </summary>
[PublicApi]
public interface IAppTyped:
        IAppIdentity
    //Eav.Apps.IApp
    //IAppPaths
{
    #region Properties basically from Eav.Apps.IApp

    /// <inheritdoc cref="Eav.Apps.IApp.Name"/>
    string Name { get; }

    /// <inheritdoc cref="Eav.Apps.IApp.Data"/>
    IAppData Data { get; }

    #endregion

    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v16.04")]
    IDataSource GetQuery(
        string name = default,
        string noParamOrder = Protector,
        IDataSourceLinkable attach = default,
        object parameters = default
    );


    /// <inheritdoc cref="IApp.Configuration"/>
    IAppConfiguration Configuration { get; }

    /// <summary>
    /// All the app settings which are custom for each app. 
    /// </summary>
    ITypedItem Settings { get;  }

    /// <summary>
    /// All the app resources (usually used for multi-language labels etc.)
    /// </summary>
    ITypedItem Resources { get;  }

    #region Paths / Urls

    IFolder Folder { get; }

    /// <summary>
    /// Get the folder of the current app, usually for creating links to assets etc.
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="location">name of the app location - either `auto` (default), `site` or `shared`</param>
    /// <returns>an IFolder object which can then use `.Url`, `.PhysicalPath` etc.</returns>
    /// <remarks>
    /// * Despite being of type `IFolder`, the object is currently not able to traverse children folders/files.
    ///   We may add this some day in future.
    /// * Previously the `Folder` property returned containing the name. This is now on `.Folder().Name`.
    /// </remarks>
    IFolder FolderAdvanced(string noParamOrder = Protector, string location = default);

    IFile Thumbnail { get; }

    #endregion

}