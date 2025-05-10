namespace ToSic.Sxc.Services;

[PrivateApi("Hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class KeyService : IKeyService
{
    private UniqueKeysServices UniqueKeysSvc => field ??= new();

    /// <inheritdoc cref="IKeyService.UniqueKey"/>
    public string UniqueKey => UniqueKeysSvc.UniqueKey;

    /// <inheritdoc cref="IKeyService.UniqueKeyOf"/>
    public string UniqueKeyOf(object data) => UniqueKeysServices.UniqueKeyOf(data);

    /// <inheritdoc cref="IKeyService.UniqueKeyWith"/>
    public string UniqueKeyWith(params object[] partners) => UniqueKeysSvc.UniqueKeyWithGen(partners);
}