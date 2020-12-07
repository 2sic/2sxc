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

    }
}
