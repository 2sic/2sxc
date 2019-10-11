using System;
using System.Text.RegularExpressions;
using ToSic.Eav.Identity;

namespace ToSic.Sxc.Adam
{
    public class Security
    {
        internal static bool PathIsInItemAdam(Guid guid, string field, string path)
        {
            var shortGuid = Mapper.GuidCompress(guid);
            // will do check, case-sensitive because the compressed guid is case-sensitive
            return path.Replace('\\', '/').Contains(shortGuid + "/" + field);
        }
    }
}
