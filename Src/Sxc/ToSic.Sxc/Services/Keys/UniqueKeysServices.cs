using ToSic.Eav.Identity;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class UniqueKeysServices
{
    internal const int UniqueKeyLength = 8;
    internal const string NullValue = "null";
    internal const string PfxBool = "b";
    internal const string PfxNum = "n";
    internal const string PfxString = "s";
    internal const string PfxHash = "hash";
    internal const string PfxEntity = "eid";
    internal const string PfxDate = "d";
    internal const string PfxGuid = "g";
    internal const string PfxUrl = "u";

    /// <summary>
    /// A unique, random key for the current module.
    /// It's recommended for giving DOM elements a unique id for scripts to then access them.
    /// 
    /// It's generated for every content-block, and more reliable than `Module.Id`
    /// since that sometimes results in duplicate keys, if the many blocks are used inside each other.
    ///
    /// It's generated using a GUID and converted/shortened. 
    /// In the current version it's 8 characters long, so it has 10^14 combinations, making collisions extremely unlikely.
    /// (currently 8 characters)
    /// </summary>
    [PrivateApi]
    public string UniqueKey => _uniqueKey ??= UniqueKeyGen();
    private string _uniqueKey;


    [PrivateApi]
    internal static string UniqueKeyGen() => Guid2UniqueKey(Guid.NewGuid());


    [PrivateApi]
    internal string UniqueKeyWithGen(object[] partners) => $"{UniqueKey}-{UniqueKeysOf(partners)}";

    internal static string UniqueKeysOf(params object[] data) => 
        data.SafeNone() ? NullValue : string.Join("-", data.Select(UniqueKeyOf));

    [PrivateApi]
    internal static string UniqueKeyOf(object data)
    {
        // Handle some initial basic cases
        switch (data)
        {
            case null: return NullValue;
            case bool b: return $"{PfxBool}{(b ? "true" : "false")}";
            case string s: return $"{PfxString}{s.GetHashCode()}";
            case DateTime d: return $"{PfxDate}{d.ToString("O").RemoveAll(':', '-', 'T', 'Z', '.').TrimEnd('0')}";
            case Guid guid: return $"{PfxGuid}{Guid2UniqueKey(guid)}";
            case ICanBeEntity canBeEntity:
                var entity = canBeEntity.Entity;
                if (entity == null) return $"{PfxEntity}{Obj2HashKey(canBeEntity)}";
                if (entity.EntityGuid != Guid.Empty) return $"{PfxEntity}{Guid2UniqueKey(entity.EntityGuid)}";
                if (entity.EntityId > 0) return $"{PfxEntity}{entity.EntityId}";
                return Obj2HashKey(entity);
            case IAsset asset:
                return $"{PfxUrl}{asset.Url.GetHashCode()}";
        }

        // handle numbers etc.
        if (data.IsNumeric()) //.GetType().UnboxIfNullable().IsNumeric())
            return PfxNum + data.ConvertOrFallback("convert-num-to-string-failed", numeric: true).Replace(".", "_");

        return Obj2HashKey(data);
    }

    private static string Guid2UniqueKey(Guid guid) => guid.GuidCompress().Substring(0, UniqueKeyLength);

    private static string Obj2HashKey(object obj) => $"{PfxHash}{obj.GetHashCode()}";

}