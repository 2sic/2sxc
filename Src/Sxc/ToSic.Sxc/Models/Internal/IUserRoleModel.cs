namespace ToSic.Sxc.Models.Internal;

public interface IUserRoleModel
{
    /// <summary>
    /// The Role ID in the database.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// The Role Name as it is displayed everywhere.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// When the user role was first created.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// When the user role was last modified.
    /// </summary>
    DateTime Modified { get; }
}