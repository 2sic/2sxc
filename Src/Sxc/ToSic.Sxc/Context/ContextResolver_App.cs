using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Context
{
    partial class ContextResolver
    {
        public IContextOfApp SetAppOrNull(string nameOrPath)
        {
            if (string.IsNullOrWhiteSpace(nameOrPath)) return null;
            var zoneId = Site().Site.ZoneId;
            var id = AppIdResolver.Value.GetAppIdFromPath(zoneId, nameOrPath, false);
            return id <= Eav.Constants.AppIdEmpty ? null : SetApp(new AppIdentity(zoneId, id));
        }
    }
}
