using System.Reflection;
using Oqtane.Models;
using Oqtane.Shared;

namespace ToSic.Sxc.Oqt.Server.Adam;

internal static class OqtFolderStorage
{
    // These constants were introduced with Oqtane 10.3; keep compiling against 10.2.
    internal const string PrivateStorageProvider = "Oqtane.Providers.PrivateStorageProvider, Oqtane.Server";
    internal const string PublicStorageProvider = "Oqtane.Providers.PublicStorageProvider, Oqtane.Server";

    private static readonly PropertyInfo StorageProperty = typeof(Folder).GetProperty("StorageProviderType")
        ?? typeof(Folder).GetProperty("Type")
        ?? throw new MissingMemberException(typeof(Folder).FullName, "Type or StorageProviderType");
    private static readonly bool UsesStorageProviders = StorageProperty.Name == "StorageProviderType";

    internal static void SetPrivateStorage(Folder folder)
        => StorageProperty.SetValue(folder, UsesStorageProviders ? PrivateStorageProvider : FolderTypes.Private);

    internal static string GetStorageProvider(Folder folder)
    {
        var storage = StorageProperty.GetValue(folder) as string;
        return UsesStorageProviders ? storage : storage switch
        {
            FolderTypes.Private => PrivateStorageProvider,
            FolderTypes.Public => PublicStorageProvider,
            _ => null
        };
    }
}
