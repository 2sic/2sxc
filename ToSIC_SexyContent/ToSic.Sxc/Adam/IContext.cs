using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Future interface, to handle the adam context
    /// The system isn't quite ready for it yet, because the SxcInstance is a critical part of the interface,
    /// and the SxcInstance shouldn't be part of the App system
    /// </summary>
    [PrivateApi]
    public interface IContext
    {
    }
}
