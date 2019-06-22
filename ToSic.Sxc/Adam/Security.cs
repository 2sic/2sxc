using System;
using System.Text.RegularExpressions;
using ToSic.Eav.Identity;

namespace ToSic.Sxc.Adam
{
    public class Security
    {
        public static Regex BadExtensions = new Regex(@"^\.\s*(ade|adp|app|bas|bat|chm|class|cmd|com|cpl|crt|dll|exe|fxp|hlp|hta|ins|isp|jse|lnk|mda|mdb|mde|mdt|mdw|mdz|msc|msi|msp|mst|ops|pcd|pif|prf|prg|reg|scf|scr|sct|shb|shs|url|vb|vbe|vbs|wsc|wsf|wsh|cshtml|vbhtml|cs|ps[0-9]|ascx|aspx|asmx|config|inc|js|html|sql|bin|iso|asp|sh|php([0-9])?|pl|cgi|386|torrent|jar|vbscript|cer|csr|jsp|drv|sys|csh|inf|htaccess|htpasswd|ksh)\s*$");

        internal static bool PathIsInItemAdam(Guid guid, string field, string path)
        {
            var shortGuid = Mapper.GuidCompress(guid);
            // will do check, case-sensitive because the compressed guid is case-sensitive
            return path.Replace('\\', '/').Contains(shortGuid + "/" + field);
        }
    }
}
