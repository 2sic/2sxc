using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// Public properties of the IUser for use in your own code
    /// </summary>
    [PublicApi]
    public interface IUserLight
    {
        /// <summary>
        /// User Id as int. Works in DNN and Oqtane
        /// </summary>
        int Id { get; }

    }
}
