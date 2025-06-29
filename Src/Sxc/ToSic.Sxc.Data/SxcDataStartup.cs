﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Data.Sys.DynamicJacket;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Data.Sys.Wrappers;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcDataStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcData(this IServiceCollection services)
    {

        // v16 DynamicJacket and DynamicRead factories
        services.TryAddTransient<ICodeDataPoCoWrapperService, CodeDataPoCoWrapperService>();
        services.TryAddTransient<CodeJsonWrapper>();
        services.TryAddTransient<WrapObjectTyped>();
        services.TryAddTransient<WrapObjectTypedItem>();

        return services;
    }


        
}