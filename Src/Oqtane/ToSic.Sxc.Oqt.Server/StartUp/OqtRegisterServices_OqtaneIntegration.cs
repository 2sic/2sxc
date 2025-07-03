﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Oqt.Server.Services;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.TurnOn.Sys;

namespace ToSic.Sxc.Oqt.Server.StartUp;

partial class OqtRegisterServices
{
    /// <summary>
    /// Mail, Logging and other services.
    /// </summary>
    private static IServiceCollection AddSxcOqtIntegratedServices(this IServiceCollection services)
    {
        services.TryAddTransient<ISystemLogService, OqtSystemLogService>();
        services.TryAddTransient<IMailService, OqtMailService>();
        services.AddTransient<ITurnOnService, OqtTurnOnService>();
        return services;
    }
}