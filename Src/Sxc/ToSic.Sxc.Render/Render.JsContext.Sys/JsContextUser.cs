using System.Text.Json.Serialization;
using ToSic.Eav.Models;
using ToSic.Sxc.Render.Polymorphism.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Render.JsContext.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class JsContextUser(IUser user, IEnumerable<IEntity>? dataList)
{
    public bool CanDevelop { get; } = user.IsSystemAdmin;

    public bool CanAdmin { get; } = user.IsSiteAdmin;

    [JsonPropertyName("canSwitchEdition")]
    public bool CanSwitchEdition { get; }
        = dataList.FirstModel<PolymorphismConfigurationModel>(
                  options: new()
                  {
                      NullHandling = NullHandling.ReturnNull
                  }
                  //nullHandling: ModelNullHandling.PreferNull
                  )
              ?.UsersWhoMaySwitch.Contains(user.Id)
          ?? false;
}