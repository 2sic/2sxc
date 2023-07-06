namespace ToSic.Sxc.WebApi
{
    public class SxcWebApiConstants
    {
        //public const string HeaderInstanceId = "moduleid";
        public const string HeaderContentBlockId = "ContentBlockId";

        // QueryStringKeys
        //public const string PageId = "pageid";
        //public const string ModuleId = "moduleid";

        /// <summary>
        /// Key for the App Folder to add in the middleware, so the Controller can find it's App
        /// </summary>
        public const string HttpContextKeyForAppFolder = "SxcAppFolderName";
    }
}
