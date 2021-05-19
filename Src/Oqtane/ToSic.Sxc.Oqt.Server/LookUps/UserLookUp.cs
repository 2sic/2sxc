using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class UserLookUp : LookUpBase
    {
        private readonly OqtUser _oqtUser;

        public UserLookUp(IUser oqtUser)
        {
            Name = "User";

            _oqtUser = oqtUser as OqtUser;
        }

        public override string Get(string key, string format)
        {
            try
            {
                return key.ToLowerInvariant() switch
                {
                    "id" => $"{_oqtUser.Id}",
                    "username" => $"{_oqtUser.UnwrappedContents.Username}",
                    "displayname" => $"{_oqtUser.UnwrappedContents.DisplayName}",
                    "email" => $"{_oqtUser.UnwrappedContents.Email}",
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
}