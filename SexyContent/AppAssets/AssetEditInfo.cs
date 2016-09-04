using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ToSic.SexyContent.AppAssets
{
    /// <summary>
    /// Information class needed by the edit-ui, to provide optimal syntax helpers etc.
    /// </summary>
    public class AssetEditInfo
    {
        
        private static readonly string[] SafeFileWhitelist = "txt,html,js,md,json,doc,docx,xls,xlsx,xml".Split(',');

        public string
            Name,
            Code,
            LocationScope = Settings.TemplateLocations.PortalFileSystem,
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

        public AssetEditInfo(int appId, string appName, string fileName)
        {
            AppId = appId;
            FileName = fileName;
            HasApp = appName != "Content";
        }

        // check if this file is safe - meaning it can be edited by non-host users
        public bool IsSafe
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FileName))
                    return true;

                var ext = Path.GetExtension(FileName);
                if (ext == "")  // no extension
                    return true;

                if (SafeFileWhitelist.Contains(ext))
                    return true;

                return false;
            }
        }

    }
}