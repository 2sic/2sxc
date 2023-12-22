using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Apps;

/// <summary>
/// Data object of an App in **Typed** mode
/// </summary>
/// <remarks>Added v17</remarks>
[PublicApi]
public interface IAppDataTyped: IDataSource
{
    #region CRUD

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Create(string, Dictionary{string, object}, string, ITarget)"/>
    IEntity Create(string contentTypeName, Dictionary<string, object> values, string userName = null, ITarget target = null);

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Create(string, IEnumerable{Dictionary{string, object}}, string)"/>
    IEnumerable<IEntity> Create(string contentTypeName, IEnumerable<Dictionary<string, object>> multiValues, string userName = null);

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Update"/>
    void Update(int entityId, Dictionary<string, object> values, string userName = null);

    /// <inheritdoc cref="ToSic.Eav.Apps.IAppData.Delete"/>
    void Delete(int entityId, string userName = null);

    #endregion

    #region ContentTypes

    // Note: Implemented as a method, so later we can apply filters etc. as additional parameters
    /// <summary>
    /// All content types of the app.
    /// </summary>
    /// <remarks>Added v17</remarks>
    IEnumerable<IContentType> GetContentTypes();

    /// <summary>
    /// Get a single content type by name (display name or NameId).
    /// </summary>
    /// <param name="name">the name, either the normal name or the NameId which looks like a GUID</param>
    /// <remarks>Added v17</remarks>
    IContentType GetContentType(string name);

    #endregion
}