using System.Globalization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Dnn.LookUp;

/// <summary>
/// Translator component which creates a LookUp object and internally accesses
/// DNN PropertyAccess objects (which DNN uses for the same concept as LookUp)
/// </summary>
internal class LookUpInDnnPropertyAccess(string name, IPropertyAccess source, UserInfo user, CultureInfo localization) : LookUpBase(name, $"LookUp in Dnn PropertyAccess source of type {source?.GetType().Name}")
{
    public override string Get(string key, string format)
    {
        var blnNotFound = false;
        var result = source.GetProperty(key, format, localization, user, Scope.DefaultSettings, ref blnNotFound);
        return blnNotFound ? string.Empty : result;
    }
}