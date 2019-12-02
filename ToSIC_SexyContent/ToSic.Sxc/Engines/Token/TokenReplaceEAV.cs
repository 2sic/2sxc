// using DotNetNuke.Entities.Portals;

using ToSic.Eav.LookUp;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Engines.Token
{
    internal class TokenReplaceEav: Eav.Tokens.TokenReplace
    {
        public int ModuleId;

        public TokenReplaceEav(int instanceId, ILookUpEngine provider)
        {
            InitAppAndPortalSettings(instanceId, provider);
        }

        public void InitAppAndPortalSettings(int moduleId, ILookUpEngine provider)
        {
            foreach (var valueProvider in provider.Sources)
                ValueSources.Add(valueProvider.Key, valueProvider.Value);

            ModuleId = moduleId;
        }

    }
}