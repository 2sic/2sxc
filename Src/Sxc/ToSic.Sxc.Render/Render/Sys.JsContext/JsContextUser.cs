using System.Text.Json.Serialization;
using ToSic.Eav.Models;
using ToSic.Sxc.Polymorphism.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Render.Sys.JsContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class JsContextUser(IUser user, IEnumerable<IEntity>? dataList)
{
    public bool CanDevelop { get; } = user.IsSystemAdmin;

    public bool CanAdmin { get; } = user.IsSiteAdmin;

    [JsonPropertyName("canSwitchEdition")]
    public bool CanSwitchEdition { get; }
        = dataList.First<PolymorphismConfiguration>(nullHandling: ModelNullHandling.PreferNull)
              ?.UsersWhoMaySwitch.Contains(user.Id)
          ?? false;
}