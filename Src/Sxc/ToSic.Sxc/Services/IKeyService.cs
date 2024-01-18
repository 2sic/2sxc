using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Services;

/// <summary>
/// WIP 16.04
/// </summary>
[PublicApi]
public interface IKeyService
{
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
    ///
    /// > [!TIP]
    /// > To get a unique key which is based on additional objects such as Entities,
    /// > use the <see cref="UniqueKeyWith"/> method.
    /// </summary>
    /// <remarks>
    /// If you get a fresh <see cref="IKeyService"/> it will also create a new UniqueKey.
    /// So your code should usually use the built-in property `UniqueKey` which comes from the shared ServiceKit <see cref="ServiceKit16.Key"/>.
    /// </remarks>
    string UniqueKey { get; }

    [PrivateApi("not yet sure if we should publish this")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    string UniqueKeyOf(object data);

    /// <summary>
    /// Generate a unique key based on the <see cref="UniqueKey"/> and additional objects.
    ///
    /// It has a special mechanisms for creating unique keys for specific data types such as entities,
    /// so calling this multiple times with the same objects will still result in the same key being generated.
    ///
    /// Special behaviors:
    ///
    /// * Strings will use the HashCode
    /// * Entities and similar will use a shortened unique string based on the GUID
    /// * Assets (files, folders) will use the HashCode of their <see cref="IAsset.Url"/>
    /// * Dates are converted to a safe string and trimmed for all trailing zeros
    /// * Most key parts will receive a simple prefix making debugging easier
    /// </summary>
    /// <param name="partners"></param>
    /// <returns></returns>
    string UniqueKeyWith(params object[] partners);
}