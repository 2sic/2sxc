namespace ToSic.Sxc.Apps.Assets
{
    /// <summary>
    /// Constants for App Assets
    /// </summary>
    internal class AppAssets
    {
        /// <summary>
        /// App file is located in the site itself.
        /// This is the Key used up until 2sxc 12.01 and will still exist in old data for a long time. 
        /// </summary>
        public static string PortalFileSystem = "Portal File System";

        /// <summary>
        /// App file is located in the shared location. 
        /// This is the Key used up until 2sxc 12.01 and will still exist in old data for a long time. 
        /// </summary>
        public static string HostFileSystem = "Host File System";

        // New terms used in 12.02+
        public static string AppInSite = "Site";
        public static string AppInGlobal = "Global";
    }
}
