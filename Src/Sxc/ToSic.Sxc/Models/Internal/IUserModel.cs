namespace ToSic.Sxc.Models.Internal;

public interface IUserModel
{
    string NameId { get; }

    ///// <summary>
    ///// Role ID List.
    ///// Important: Internally we use a list to do checks etc.
    ///// But for creating the entity we return a CSV
    ///// </summary>
    //List<int> Roles { get; }

    bool IsSystemAdmin { get; }
    bool IsSiteAdmin { get; }
    bool IsContentAdmin { get; }
    bool IsContentEditor { get; }
    bool IsSiteDeveloper { get; }
    bool IsAnonymous { get; }
    string Username { get; }
    string Email { get; } // aka PreferredEmail
    string Name { get; } // aka DisplayName
    int Id { get; }
    Guid Guid { get; }
    DateTime Created { get; }
    DateTime Modified { get; }
}