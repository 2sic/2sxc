using ToSic.Eav.Context;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.Metadata;

namespace ToSic.Eav.Apps;

/// <summary>
/// An App in memory - for quickly getting things done with the app data, queries etc.
/// </summary>
[PublicApi]
public interface IApp : IAppIdentity, IHasMetadata
{
    /// <summary>
    /// App Name
    /// </summary>
    /// <returns>The name as configured in the app configuration.</returns>
    string Name { get; }

    /// <summary>
    /// App Folder
    /// </summary>
    /// <returns>The folder as configured in the app configuration.</returns>
    string Folder { get; }

    /// <summary>
    /// NameId of the App - usually a string-GUID
    /// </summary>
    string NameId { get; }

    [PrivateApi]
    [Obsolete("Don't use any more, use NameId instead, will be removed ca. v14")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    string AppGuid { get; }

    /// <summary>
    /// Data of the app
    /// </summary>
    IAppData Data { get; }

    /// <summary>
    /// The app metadata - like settings, resources etc.
    /// </summary>
    /// <returns>A metadata provider for the app</returns>
    new IMetadataOf Metadata { get; }


    #region Experimental / new


    /// <summary>
    /// The tenant this app belongs to - for example, a DNN portal
    /// </summary>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    [Obsolete("Don't use any more, use NameId instead, will be removed ca. v17")]
    ISite Site { get; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    [Obsolete("Don't use any more?? note: 2026-06 2dm not sure if this is really deprecated, used to say: use NameId instead, will be removed ca. v14")]
    // TODO: MARK as #Deprecated and log access
    ILookUpEngine ConfigurationProvider { get; }

    #endregion
}