using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// The user context of your code - so it's information about the user your code is using. 
    /// </summary>
    [PublicApi]
    public interface ICmsUser: IHasMetadata
    {
        /// <summary>
        /// User Id as int. Works in DNN and Oqtane.
        /// Use in Razor: `CmsContext.User.Id`
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Information if the user is anonymous (not logged in)
        /// </summary>
        /// <remarks>
        /// This was added fairly late in v14.08
        /// </remarks>
        bool IsAnonymous { get; }

        /// <summary>
        /// Information if the current user is Site Administrator.
        /// Basically this means a user has very high permissions - incl. the ability
        /// to create users in a site etc.
        /// 
        /// 🪒 Use in Razor: `CmsContext.User.IsSiteAdmin`
        /// </summary>
        /// <remarks>
        /// These are not the highest possible privileges
        /// - For the site it would be IsSiteDeveloper
        /// - For the entire system that would be IsSystemAdmin.
        /// New in 2sxc 12
        /// </remarks>
        bool IsSiteAdmin { get; }

        /// <summary>
        /// Information if the current user is System Administrator.
        /// Basically this means a user has maximum permissions - incl. the ability
        /// to install additional components or do dangerous things like edit razor.
        /// 
        /// 🪒 Use in Razor: `CmsContext.User.isSystemAdmin`
        /// </summary>
        /// <remarks>
        /// New in 2sxc 12
        /// </remarks>
        bool IsSystemAdmin { get; }


        /// <summary>
        /// Information if the current user is Developer on the current site.
        /// Basically this means a user has maximum site permissions - incl. the ability
        /// to install additional components or do dangerous things like edit razor.
        /// 
        /// 🪒 Use in Razor: `CmsContext.User.IsSiteDeveloper`
        /// </summary>
        /// <remarks>
        /// These are not the highest possible privileges
        /// - For the entire system that would be IsSystemAdmin.
        /// New in 2sxc 12
        /// </remarks>
        bool IsSiteDeveloper { get; }

        /// <summary>
        /// Metadata of the current view
        /// </summary>
        /// <remarks>
        /// Added in v13.12
        /// </remarks>
#pragma warning disable CS0108, CS0114
        IDynamicMetadata Metadata { get; }
#pragma warning restore CS0108, CS0114

    }
}
