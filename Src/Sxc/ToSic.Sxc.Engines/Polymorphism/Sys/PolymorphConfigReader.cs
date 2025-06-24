﻿using ToSic.Eav.Apps;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.Polymorphism.Internal;

/// <summary>
/// Mini service to read the polymorph config of the app
/// and then resolve the edition based on an <see cref="IPolymorphismResolver"/>
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class PolymorphConfigReader(LazySvc<ServiceSwitcher<IPolymorphismResolver>> resolvers, LazySvc<IHttp> http) : ServiceBase("Plm.Managr", connect: [resolvers ])
{
    public PolymorphismConfiguration Configuration = null!;
    private int _appId;

    /// <summary>
    /// Helper to either use the edition set by the view (like in preview mode)
    /// or to use this same PolymorphConfigReader to figure out the edition.
    /// Since the reader should only be created if necessary, it's handed in as a function.
    /// </summary>
    public string? UseViewEditionOrGet(IBlock block)
        => !block.ViewIsReady
            ? null
            : UseViewEditionOrGet(block.View, block.Context.AppReaderRequired);

    public string? UseViewEditionOrGet(IView? view, IAppReader? appReader)
    {
        var viewEdition = view?.Edition.NullIfNoValue();
        if (viewEdition != null)
            return viewEdition;

        // note: up until v20 2025-06-19 this always worked without null-checking the app-reader.
        // so for now, we'll assume all paths always work, which is why we use appReader! here.
        _appId = appReader!.AppId;
        Init(appReader.List);
        return Edition();
    }

    public PolymorphConfigReader Init(IEnumerable<IEntity> list)
    {
        Configuration = new(list);
        return this;
    }

    public string? Edition()
    {
        var l = Log.Fn<string?>();
        try
        {
            var resolver = Configuration.Resolver;
            if (resolver.IsEmpty()) 
                return l.ReturnNull("no resolver");

            var rInfo = resolvers.Value.ByNameId(resolver, true);
            if (rInfo == null)
                return l.ReturnNull("resolver not found");
            l.A($"resolver for {resolver} found");
            var overrule = http.Value.GetCookie($"app-{_appId}-edition").NullIfNoValue();
            l.A($"overrule: '{overrule}'");
            var result = rInfo.Edition(Configuration, overrule, Log);

            return l.Return(result, "ok");
        }
        // We don't expect errors - but such a simple helper just shouldn't be able to throw errors
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("error");
        }
    }

}