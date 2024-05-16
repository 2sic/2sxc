using System.Text.Json.Serialization;
using ToSic.Eav.Context;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Web.Internal.JsContext;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsContextUser(IUser user, IEnumerable<IEntity> dataList)
{
    public bool CanDevelop { get; } = user.IsSystemAdmin;

    public bool CanAdmin { get; } = user.IsSiteAdmin;

    [JsonPropertyName("canSwitchEdition")]
    public bool CanSwitchEdition { get; } = new PolymorphismConfiguration(dataList).UsersWhoMaySwitch.Contains(user.Id);
}