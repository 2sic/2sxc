// using DotNetNuke.Entities.Portals;

using ToSic.Eav.LookUp;
using ToSic.SexyContent;

namespace ToSic.Sxc.Engines.Token
{
    internal class TokenReplaceEav: Eav.Tokens.TokenReplace
    {
        public int ModuleId;

        public TokenReplaceEav(App app, int instanceId, ITokenListFiller provider)
        {
            InitAppAndPortalSettings(app, instanceId, provider);
        }

        public void InitAppAndPortalSettings(App app, int moduleId, ITokenListFiller provider)
        {
            foreach (var valueProvider in provider.Sources)
                ValueSources.Add(valueProvider.Key, valueProvider.Value);

            ModuleId = moduleId;
        }

    }
}