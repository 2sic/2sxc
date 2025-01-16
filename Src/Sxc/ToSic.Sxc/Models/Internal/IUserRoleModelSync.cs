namespace ToSic.Sxc.Models.Internal;

public interface IUserRoleModelSync
{
    /// <summary>
    /// The Role ID in the database.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The Role Name as it is displayed everywhere.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// When the user role was first created.
    /// </summary>
    public DateTime Created { get; }

    /// <summary>
    /// When the user role was last modified.
    /// </summary>
    public DateTime Modified { get; }
}