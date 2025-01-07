namespace ToSic.Sxc.Models.Internal;

public interface IUserRoleModel
{
    string Name { get; }
    int Id { get; }
    DateTime Created { get; }
    DateTime Modified { get; }
}