using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.Documentation;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Dnn.LookUp
{
    /// <summary>
    /// Translator component which creates a LookUp object and internally accesses
    /// DNN PropertyAccess objects (which DNN uses for the same concept as LookUp)
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class LookUpInDnnPropertyAccess: LookUpBase
    {
        private readonly IPropertyAccess _source;
        private readonly UserInfo _user;
        private readonly CultureInfo _loc;

        public LookUpInDnnPropertyAccess(string name, IPropertyAccess source, UserInfo user, CultureInfo localization)
        {
            Name = name;
            _source = source;
            _user = user;
            _loc = localization;
        }

        public override string Get(string key, string format)
        {
            var blnNotFound = false;
            var result = _source.GetProperty(key, format, _loc, _user, Scope.DefaultSettings, ref blnNotFound);
            return blnNotFound ? string.Empty : result;
        }
    }
}