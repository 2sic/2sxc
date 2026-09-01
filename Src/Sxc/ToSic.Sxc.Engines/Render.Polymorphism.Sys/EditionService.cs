using ToSic.Eav.Apps;
using ToSic.Eav.Models;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Web.Sys.Http;

namespace ToSic.Sxc.Render.Polymorphism.Sys;

/// <inheritdoc cref="IEditionService"/>
/// <param name="resolvers">Service switcher for polymorphism resolvers.</param>
/// <param name="http">Http service to check if a cooking specifies another edition.</param>
internal class EditionService(Generator<IPolymorphismResolver> resolvers, LazySvc<IHttp> http)
    : ServiceBase("Plm.Managr", connect: [resolvers ]), IEditionService
{
    /// <inheritdoc/>
    public string? Edition(IBlock block)
        => !block.ViewIsReady
            ? null
            : Edition(block.View, block.Context.AppReaderRequired);

    /// <inheritdoc/>
    public string? Edition(IView? view, IAppReader appReader)
        => view?.Edition.NullIfNoValue()
           ?? Edition(appReader);

    private string? Edition(IAppReader appReader)
    {
        var l = Log.Fn<string?>();
        
        try
        {
            var configuration = appReader.List.FirstModel<PolymorphismConfigurationModel>(
                options: new() { NullHandling = NullHandling.ReturnModel }
            )!;

            if (configuration.Resolver.IsEmpty()) 
                return l.ReturnNull("no resolver");

            var resolver = resolvers.TryNew(configuration.Resolver.ToLowerInvariant());
            if (resolver == null)
                return l.ReturnNull("resolver not found");
            
            var overrule = http.Value
                .GetCookie($"app-{appReader.AppId}-edition")
                .NullIfNoValue();
            var result = resolver.Edition(configuration, overrule, Log);

            return l.Return(result, $"resolver for {configuration.Resolver} found; overrule: '{overrule}'; edition: {result}");
        }
        // We don't expect errors - but such a simple helper just shouldn't be able to throw errors
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("error");
        }
    }
}