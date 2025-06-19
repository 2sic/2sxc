﻿using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Web.Internal.JsContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class JsContextLanguage(LazySvc<IZoneMapper> zoneMapperLazy)
{
    public string? Current { get; private set; }
    public string? Primary { get; private set; }
    public IEnumerable<ClientInfoLanguage>? All { get; private set; }

    public JsContextLanguage Init(ISite site)
    {
        Current = site.CurrentCultureCode;
        Primary = site.DefaultCultureCode;
        All = zoneMapperLazy.Value
            .CulturesEnabledWithState(site) // .Where(c => c.IsEnabled)
            .Select(c => new ClientInfoLanguage { key = c.Code.ToLowerInvariant(), name = c.Culture });
        return this;
    }
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ClientInfoLanguage
{
    // key and name must be lowercase, has side effects in EAV
    // ReSharper disable InconsistentNaming
    public string? key;
    public string? name;
    // ReSharper restore InconsistentNaming
}