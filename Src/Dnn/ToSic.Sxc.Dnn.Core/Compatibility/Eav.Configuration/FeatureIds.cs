using ToSic.Eav.Internal.Features;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Configuration;

/// <summary>
/// Note: these values are possibly used in published Apps - not often, but it's possible.
/// The apps would use this to check if one of the older features existed.
/// Just make sure we don't use them in our code. 
/// </summary>
[PrivateApi("this should probably never be public, as we want to rename things at will")]
[Obsolete]
public class FeatureIds
{
    // Important: these names are public - don't ever change them
    public static Guid PublicForms => BuiltInFeatures.PublicEditForm.Guid;
    public static Guid PublicUpload => BuiltInFeatures.PublicUploadFiles.Guid;
    public static Guid UseAdamInWebApi => BuiltInFeatures.SaveInAdamApi.Guid;
    public static Guid PermissionCheckUserId => BuiltInFeatures.PermissionCheckUsers.Guid;
    public static Guid PermissionCheckGroups => BuiltInFeatures.PermissionCheckGroups.Guid;
}