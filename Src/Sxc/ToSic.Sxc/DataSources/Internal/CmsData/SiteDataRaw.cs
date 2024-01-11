using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Internal class to hold all the information about the site,
/// until it's converted to an IEntity in the <see cref="Sites"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * [Dnn Site](TODO:https://docs.dnncommunity.org/api/DotNetNuke.Entities.Portals.PortalInfo.html)
/// * [Oqtane Site](TODO:https://docs.oqtane.org/api/Oqtane.Models.Sites.html)
/// 
/// Important: this is an internal object.
/// We're just including in in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// </remarks>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SiteDataRaw: IRawEntity
{
    internal static string TypeName = "Site";
    internal static DataFactoryOptions Options => new(typeName: TypeName, titleField: nameof(Name), autoId: false);

    /// <summary>
    /// The site ID.
    ///
    /// * In Dnn it's from `PortalInfo.PortalID`
    /// * In Oqtane it's `Site.SiteId`
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The site GUID.
    ///
    /// * In Dnn it's from `PortalInfo.GUID`
    /// * In Oqtane it's `Guid.Empty` as Oqtane doesn't have site GUIDs
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// The site name.
    ///
    /// * In Dnn it's from `PageInfo.PortalName`
    /// * in Oqtane it's from `Site.Name`
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The public url to this site (without any trailing slashes)
    ///
    /// * In Dnn it's from `PortalAliasInfo.FullUrl` (last slash removed)
    /// * in Oqtane it's a combination of protocol, site-alias and path
    /// </summary>
    public string Url { get; set; }


    /// <summary>
    /// The site languages, comma separated.
    /// Can be empty ever if a <see cref="DefaultLanguage"/> is set, if the site itself is not multi-language.
    /// </summary>
    public string Languages { get; set; }


    /// <summary>
    /// The site Culture Code.
    ///
    /// * In Dnn it's from `PortalInfo.CultureCode`
    /// * in Oqtane it's from `Site.CultureCode`
    /// </summary>
    public string DefaultLanguage { get; set; }

    // 2023-02-13 2dm disabled this. It's a very exotic property, don't think it should be in the normal data source
    ///// <summary>
    ///// Determines if visitors may register / create user accounts
    /////
    ///// * In Dnn it's from `PortalInfo.UserRegistration`
    ///// * in Oqtane it's from `Site.AllowRegistration`
    ///// </summary>
    //public bool AllowRegistration { get; set; }

    /// <summary>
    /// The site creation date/time.
    ///
    /// * In Dnn it's from `PortalInfo.CreatedOnDate`
    /// * in Oqtane it's from `Site.CreatedOn`
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// The site modification date/time.
    ///
    /// * In Dnn it's from `PortalInfo.LastModifiedOnDate`
    /// * in Oqtane it's from `Site.ModifiedOn`
    /// </summary>
    public DateTime Modified { get; set; }


    public int ZoneId { get; set; }

    /// <summary>
    /// The Content App.
    /// </summary>
    public int ContentAppId { get; set; }

    /// <summary>
    /// The (technical) primary App, which contains things such as site metadata.
    /// </summary>
    public int PrimaryAppId { get; set; }

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    public IDictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>
    {
        { nameof(Name), Name },
        { nameof(Url), Url },
        { nameof(Languages), Languages },
        { nameof(DefaultLanguage), DefaultLanguage },
        { nameof(ZoneId), ZoneId },
        { nameof(ContentAppId), ContentAppId },
        { nameof(PrimaryAppId), PrimaryAppId },
    };

}