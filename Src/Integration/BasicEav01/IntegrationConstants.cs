using ToSic.Eav.Apps;

// #2sxcIntegration

namespace IntegrationSamples.BasicEav01
{
    /// <summary>
    /// This contains constant parameters for the integration demo to work.
    /// You will probably adjust these to match your needs
    /// </summary>
    public class IntegrationConstants
    {
        /// <summary>
        /// In the demo setup this is the blog app on the PC of 2dm
        /// </summary>
        public const int ZoneId = 2;
        public const int AppId = 78;

        public static IAppIdentity AppIdentity = new AppIdentity(ZoneId, AppId);
    }
}
