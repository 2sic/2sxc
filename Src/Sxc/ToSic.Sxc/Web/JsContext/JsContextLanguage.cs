using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Web.JsContext
{
    public class JsContextLanguage
    {
        public string Current { get; private set; }
        public string Primary { get; private set; }
        public IEnumerable<ClientInfoLanguage> All { get; private set; }

        public JsContextLanguage(ILazySvc<IZoneMapper> zoneMapperLazy) => _zoneMapperLazy = zoneMapperLazy;
        private readonly ILazySvc<IZoneMapper> _zoneMapperLazy;

        public JsContextLanguage Init(ISite site, int zoneId)
        {
            Current = site.CurrentCultureCode;
            Primary = site.DefaultCultureCode;
            All = _zoneMapperLazy.Value
                .CulturesWithState(site)
                .Where(c => c.IsEnabled)
                .Select(c => new ClientInfoLanguage { key = c.Code.ToLowerInvariant(), name = c.Culture });
            return this;
        }
    }

    public class ClientInfoLanguage
    {
        // key and name must be lowercase, has side effects in EAV
        // ReSharper disable InconsistentNaming
        public string key;
        public string name;
        // ReSharper restore InconsistentNaming
    }
}
