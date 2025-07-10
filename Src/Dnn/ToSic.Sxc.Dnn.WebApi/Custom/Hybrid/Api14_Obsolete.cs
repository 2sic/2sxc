using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

partial class Api14
{
    // Obsolete stuff - not supported anymore in after V10 - show helpful error messages

    #region Shared Code Block between RazorComponent_Obsolete and ApiController_Obsolete

    #region Obsolete CreateSource

    [PrivateApi]
    [Obsolete("throws error with fix-instructions. Use CreateSource<type> instead.")]
    public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null)
        => RazorExceptions.ExCreateSourceString();

    #endregion

    #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10 - Removed in v20

    //[PrivateApi]
    //[Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
    //public dynamic AsDynamic(ToSic.Eav.Interfaces.IEntity entity)
    //    => RazorExceptions.ExAsDynamicInterfacesIEntity();


    //[PrivateApi]
    //[Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
    //public dynamic AsDynamic(KeyValuePair<int, ToSic.Eav.Interfaces.IEntity> entityKeyValuePair)
    //    => RazorExceptions.AsDynamicKvpInterfacesIEntity();

    //[Obsolete("throws error with fix-instructions. Cast your entities to ToSic.Eav.Data.IEntity")]
    //[PrivateApi]
    //public IEnumerable<dynamic> AsDynamic(IEnumerable<ToSic.Eav.Interfaces.IEntity> entities)
    //    => RazorExceptions.AsDynamicIEnumInterfacesIEntity();


    #endregion

    #region AsDynamic<int, IEntity>

    [PrivateApi]
    [Obsolete("throws error with fix-instructions. Use AsDynamic(IEnumerable<IEntity>...)")]
    public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => RazorExceptions.ExAsDynamicKvp();

    #region Old AsDynamic with correct warnings
    /// <inheritdoc/>
    [PrivateApi]
    public IEnumerable<dynamic> AsDynamic(IDataStream stream)
        => throw new($"AsDynamic for lists isn't supported here. Please use AsList(...) instead.");

    /// <inheritdoc/>
    [PrivateApi]
    public IEnumerable<dynamic> AsDynamic(IDataSource source)
        => throw new($"AsDynamic for lists isn't supported here. Please use AsList(...) instead.");


    /// <inheritdoc/>
    [PrivateApi]
    public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities)
        => throw new($"AsDynamic for lists isn't supported here. Please use AsList(...) instead.");

    #endregion
    #endregion

    #region Presentation, ListContent, ListPresentation, List

    [PrivateApi]
    [Obsolete("use Content.Presentation instead")]
    public dynamic Presentation => RazorExceptions.ExPresentation();


    [PrivateApi]
    [Obsolete("Use Header instead")]
    public dynamic ListContent => RazorExceptions.ExListContent();

    [PrivateApi]
    [Obsolete("Use Header.Presentation instead")]
    public dynamic ListPresentation => RazorExceptions.ExListPresentation();

    [PrivateApi]
    [Obsolete("This is an old way used to loop things - removed in RazorComponent")]
    public IEnumerable<dynamic> List => RazorExceptions.ExList();

    #endregion

    #endregion

}