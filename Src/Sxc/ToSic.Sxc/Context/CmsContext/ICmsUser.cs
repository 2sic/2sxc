using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// The user context of your code - so it's information about the user your code is using. 
    /// </summary>
    [PublicApi]
    public interface ICmsUser
    {
        /// <summary>
        /// User Id as int. Works in DNN and Oqtane
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Information if the current user is Site Administrator.
        /// Basically this means a user has very high permissions - incl. the ability
        /// to create users in a site etc.
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
        /// </summary>
        /// <remarks>
        /// New in 2sxc 12
        /// </remarks>
        bool IsSystemAdmin { get; }


        /// <summary>
        /// Information if the current user is Developer on the current site.
        /// Basically this means a user has maximum site permissions - incl. the ability
        /// to install additional components or do dangerous things like edit razor. 
        /// </summary>
        /// <remarks>
        /// These are not the highest possible privileges
        /// - For the entire system that would be IsSystemAdmin.
        /// New in 2sxc 12
        /// </remarks>
        bool IsSiteDeveloper { get; }
    }
}
