namespace ToSic.Sxc.Backend;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcWebApiConstants
{
    //public const string HeaderInstanceId = "moduleid";
    public const string HeaderContentBlockId = "ContentBlockId";

    public const string HeaderContentBlockList = "BlockIds";

    // QueryStringKeys
    //public const string PageId = "pageid";
    //public const string ModuleId = "moduleid";

    /// <summary>
    /// Key for the App Folder to add in the middleware, so the Controller can find it's App
    /// </summary>
    public const string HttpContextKeyForAppFolder = "SxcAppFolderName";
}