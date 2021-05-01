using System;
using System.Collections.Generic;
using Oqtane.Models;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Oqt.Shared.Dev
{
    /// <summary>
    /// Temporary constants which should be removed soon
    /// </summary>
    public class WipConstants
    {
        public static string DefaultLanguage => "en-us";
        public static Dictionary<string,string> EmptyLanguages = new Dictionary<string, string>();
        public static List<TempTempCulture> EmptyCultureList = new List<TempTempCulture>();

        //public static string ContentRoot = "/content-wip/";

        // ADAM
        public const int MaxUploadSize = 25000;
        public const int ParentFolderNotFound = 0;
        public static void AdamNotImplementedYet() { }
       

        public static void DontDoAnythingImplementLater() { }

        // Settings
        public const string SettingsChangeUserId = "sxc-auto";
    }
}
