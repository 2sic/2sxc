// using DotNetNuke.Entities.Portals;

using ToSic.Eav.LookUp;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Engines.Token
{
    internal class TokenReplaceEav: TokenReplace
    {
        public int ModuleId;

        public TokenReplaceEav(int instanceId, ILookUpEngine lookUpEngine): base(lookUpEngine)
            => InitAppAndPortalSettings(instanceId/*, lookUpEngine*/);

        private void InitAppAndPortalSettings(int moduleId/*, ILookUpEngine provider*/)
        {
            //foreach (var valueProvider in provider.Sources)
            //    //ValueSources.Add(valueProvider.Key, valueProvider.Value);
            //    LookupEngine.Add(valueProvider.Value);

            ModuleId = moduleId;
        }

    }
}