namespace ToSic.Sxc.Services;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class KeyService : IKeyService
{
    private UniqueKeysServices UniqueKeysSvc => _uniqueKeysServices ??= new();
    private UniqueKeysServices _uniqueKeysServices;

    /// <inheritdoc cref="IKeyService.UniqueKey"/>
    public string UniqueKey => UniqueKeysSvc.UniqueKey;

    /// <inheritdoc cref="IKeyService.UniqueKeyOf"/>
    public string UniqueKeyOf(object data) => UniqueKeysServices.UniqueKeyOf(data);

    /// <inheritdoc cref="IKeyService.UniqueKeyWith"/>
    public string UniqueKeyWith(params object[] partners) => UniqueKeysSvc.UniqueKeyWithGen(partners);
}