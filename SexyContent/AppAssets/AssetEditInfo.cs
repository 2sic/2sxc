using System.Collections.Generic;

namespace ToSic.SexyContent.AppAssets
{
    /// <summary>
    /// Information class needed by the edit-ui, to provide optimal syntax helpers etc.
    /// </summary>
    public class AssetEditInfo
    {
        public string
            Name,
            Code,
            FileName,
            TypeContent,
            TypeContentPresentation,
            TypeList,
            TypeListPresentation;

        public string Type = "Token";
        public bool HasList;
        public bool HasApp;
        public int AppId;
        public Dictionary<string, string> Streams = new Dictionary<string, string>();

    }
}