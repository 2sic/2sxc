using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Engines.Token
{
    internal class TokenReplaceEav: TokenReplace
    {
        public TokenReplaceEav(ILookUpEngine lookUpEngine)
            : base(lookUpEngine) { }
    }
}