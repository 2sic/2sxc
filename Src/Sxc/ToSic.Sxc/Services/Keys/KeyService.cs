
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    internal class KeyService : IKeyService
    {
        private UniqueKeysServices UniqueKeysSvc => _uniqueKeysServices ?? (_uniqueKeysServices = new UniqueKeysServices());
        private UniqueKeysServices _uniqueKeysServices;

        /// <inheritdoc cref="IKeyService.UniqueKey"/>
        public string UniqueKey => UniqueKeysSvc.UniqueKey;

        /// <inheritdoc cref="IKeyService.UniqueKeyOf"/>
        public string UniqueKeyOf(object data) => UniqueKeysServices.UniqueKeyOf(data);

        /// <inheritdoc cref="IKeyService.UniqueKeyWith"/>
        public string UniqueKeyWith(params object[] partners) => UniqueKeysSvc.UniqueKeyWithGen(partners);
    }
}
