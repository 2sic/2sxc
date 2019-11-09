// using DotNetNuke.Entities.Portals;

using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Engines.Token
{
    internal class TokenReplaceEav: Eav.Tokens.TokenReplace
    {
        public int ModuleId;

        public TokenReplaceEav(int instanceId, ITokenListFiller provider)
        {
            InitAppAndPortalSettings(instanceId, provider);
        }

        public void InitAppAndPortalSettings(int moduleId, ITokenListFiller provider)
        {
            foreach (var valueProvider in provider.Sources)
                ValueSources.Add(valueProvider.Key, valueProvider.Value);

            ModuleId = moduleId;
        }

    }
}