using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Raw;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.DataSources
{
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
    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    public class CmsSiteInfo: IRawEntity
    {
        /// <summary>
        /// The site ID.
        ///
        /// * In Dnn it's from `PortalInfo.PortalID`
        /// * In Oqtane it's `Site.SiteId`
        /// </summary>
        public int Id { get; set; } = Eav.Constants.NullId; // 0 is valid Id

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
        /// The site languages.
        /// </summary>
        public Dictionary<string, string> Languages { get; set; }


        /// <summary>
        /// The site Culture Code.
        ///
        /// * In Dnn it's from `PortalInfo.CultureCode`
        /// * in Oqtane it's from `Site.CultureCode`
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Determines if visitors may register / create user accounts
        ///
        /// * In Dnn it's from `PortalInfo.UserRegistration`
        /// * in Oqtane it's from `Site.AllowRegistration`
        /// </summary>
        public bool AllowRegistration { get; set; }

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
        public int DefaultAppId { get; set; }
        //public int ContentAppId { get; set; } // TODO: ask 2DM ContentAppId vs. DefaultAppId (is this the same)
        public int PrimaryAppId { get; set; }

        /// <summary>
        /// Data but without Id, Guid, Created, Modified
        /// </summary>
        [PrivateApi]
        public Dictionary<string, object> RawProperties => new Dictionary<string, object>
        {
            { Attributes.TitleNiceName, Name },
            { nameof(Name), Name },
            { nameof(Url), Url },
            { nameof(Languages), Languages },
            { nameof(LanguageCode), LanguageCode },
            { nameof(AllowRegistration), AllowRegistration },
            { nameof(ZoneId), ZoneId },
            { nameof(DefaultAppId), DefaultAppId },
            //{ nameof(ContentAppId), ContentAppId },
            { nameof(PrimaryAppId), PrimaryAppId }
        };
    }
}