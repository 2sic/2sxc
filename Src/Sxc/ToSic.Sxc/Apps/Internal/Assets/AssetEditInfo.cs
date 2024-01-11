using System.IO;

namespace ToSic.Sxc.Apps.Internal.Assets;

/// <summary>
/// Information class needed by the edit-ui, to provide optimal syntax helpers etc.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssetEditInfo
{
        
    private static readonly string[] SafeFileWhitelist = "txt,html,js,md,json,doc,docx,xls,xlsx,xml".Split(',');

    public string
        Name,
        Code,
        FileName,
        TypeContent,
        TypeContentPresentation,
        TypeList,
        TypeListPresentation;

    public string Type = "Auto";
    public bool HasList;
    public bool HasApp;
    public int AppId;
    public bool IsShared;
    public Dictionary<string, string> Streams = new();

    public string Extension => Path.GetExtension(FileName);

    /// <summary>
    /// parameter-less constructor for deserialization, to fix "System.InvalidOperationException"
    /// "Each parameter in the deserialization constructor on type 'ToSic.Sxc.Apps.Assets.AssetEditInfo' must bind to an object property or field on deserialization.
    /// Each parameter name must match with a property or field on the object. The match can be case-insensitive.",
    /// </summary>
    public AssetEditInfo() { }

    public AssetEditInfo(int appId, string appName, string fileName, bool global)
    {
        AppId = appId;
        FileName = fileName;
        HasApp = appName != Eav.Constants.ContentAppName;
        IsShared = global;
    }

    // check if this file is safe - meaning it can be edited by non-host users
    public bool IsSafe
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return true;

            var ext = Extension;
            if (ext == "")  // no extension
                return true;

            if (SafeFileWhitelist.Contains(ext))
                return true;

            return false;
        }
    }

}