
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

using ToSic.Sxc.Services;

namespace ToSic.Sxc.Context.Keys
{
    internal class KeyService : IKeyService
    {
        private UniqueKeysServices UniqueKeysSvc => _uniqueKeysServices ?? (_uniqueKeysServices = new UniqueKeysServices());
        private UniqueKeysServices _uniqueKeysServices;

        public string UniqueKey => UniqueKeysSvc.UniqueKey;

        public string UniqueKeyWith(params object[] partners) => UniqueKeysSvc.UniqueKeyWithGen(partners);
    }
}
