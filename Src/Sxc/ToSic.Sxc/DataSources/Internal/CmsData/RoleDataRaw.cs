using ToSic.Eav.Context;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Internal class to hold all the information about the role.
/// until it's converted to an IEntity in the <see cref="Roles"/> DataSource.
///
/// For detailed documentation, check the docs of the underlying objects:
///
/// * TODO:
/// * TODO:
/// Important: this is an internal object.
/// We're just including in in the docs to better understand where the properties come from.
/// We'll probably move it to another namespace some day.
/// </summary>
/// <remarks>
/// Make sure the property names never change, as they are critical for the created Entity.
/// </remarks>
[PrivateApi("Was InternalApi till v17 - hide till we know how to handle to-typed-conversions")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RoleDataRaw: RawEntityBase, IRawEntity, IRole
{
    internal static string TypeName = "Role";
    internal static string TitleFieldName = nameof(Name);

    internal static DataFactoryOptions Options = new(typeName: "Role", titleField: nameof(Name), autoId: false);

    public string Name { get; set; }

    /// <summary>
    /// Data but without Id, Guid, Created, Modified
    /// </summary>
    [PrivateApi]
    public override IDictionary<string, object> Attributes(RawConvertOptions options) => new Dictionary<string, object>
    {
        { nameof(Name), Name },
    };
}