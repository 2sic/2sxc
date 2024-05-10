using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Oqt.Server.Context;

namespace ToSic.Sxc.Oqt.Server.LookUps;

internal class OqtUserLookUp(IUser oqtUser) : LookUpBase(LookUpConstants.SourceUser, "LookUp in Oqtane User")
{
    private readonly OqtUser _oqtUser = oqtUser as OqtUser;

    public override string Get(string key, string format)
    {
        try
        {
            return key.ToLowerInvariant() switch
            {
                "id" => $"{_oqtUser.Id}",
                "username" => $"{_oqtUser.GetContents().Username}",
                "displayname" => $"{_oqtUser.GetContents().DisplayName}",
                "email" => $"{_oqtUser.GetContents().Email}",
                "guid" => $"{_oqtUser.Guid}",

                //"issuperuser" => $"{_oqtUser.IsSuperUser}",
                //"isadmin" => $"{_oqtUser.IsAdmin}",
                //"isanonymous" => $"{_oqtUser.IsAnonymous}",

                _ => string.Empty
            };
        }
        catch
        {
            return string.Empty;
        }
    }
}