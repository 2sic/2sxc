using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Server.StartUp;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.DI;
using ToSic.Sys.Logging;
using static Xunit.Assert;

namespace ToSic.Sxc.Oqt.StartUp;

public class OqtViewBuilderRegistrationTests
{
    [Fact]
    public void BuildServiceProvider_ValidatesViewBuilder_WithoutDirectBlockContextRegistration()
    {
        var registrations = new ServiceCollection().AddSxcOqtane();
        IServiceCollection services = new ServiceCollection();
        foreach (var serviceType in new[] { typeof(IOqtSxcViewBuilder), typeof(OqtContextOfBlockFactory) })
            services.Add(registrations.Single(descriptor => descriptor.ServiceType == serviceType));

        services.AddTransient(typeof(Generator<>), typeof(Generator<>));
        services.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
        // These leaf services are outside this startup-validation test and are never instantiated.
        services.AddScoped<ISite>(_ => throw new NotSupportedException());
        services.AddScoped<IPage>(_ => throw new NotSupportedException());
        services.AddTransient<OqtPageOutput>(_ => throw new NotSupportedException());
        services.AddTransient<BlockOfModule>(_ => throw new NotSupportedException());
        services.AddScoped<ISxcCurrentContextService>(_ => throw new NotSupportedException());
        services.AddTransient<ILogStore>(_ => throw new NotSupportedException());
        services.AddTransient<GlobalTypesCheck>(_ => throw new NotSupportedException());
        services.AddTransient<IOutputCache>(_ => throw new NotSupportedException());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });

        Null(provider.GetService<IContextOfBlock>());
    }
}
