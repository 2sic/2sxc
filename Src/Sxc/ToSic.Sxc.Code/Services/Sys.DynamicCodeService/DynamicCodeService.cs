using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Services.Sys.CodeApiServiceHelpers;

namespace ToSic.Sxc.Services.Sys.DynamicCodeService;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class DynamicCodeService(CodeApiServiceBase.Dependencies services, string? logName = null /* must be nullable for DI */)
    : CodeApiServiceBase(services, logName ?? $"{SxcLogName}.DCS"),
#pragma warning disable CS0618 // Type or member is obsolete
        IDynamicCodeService
#pragma warning restore CS0618 // Type or member is obsolete
{
    #region Constructor and Init

    /// <summary>
    /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects.
    /// It's important to understand that everything in here will use the scoped service provider.
    /// </summary>
    [field: AllowNull, MaybeNull]
    protected IServiceProvider ScopedServiceProvider => field ??= Services.ServiceProvider.CreateScope().ServiceProvider;

    [field: AllowNull, MaybeNull]
    private ScopedDependencies ServicesScoped => field ??= ScopedServiceProvider.Build<ScopedDependencies>().ConnectServices(Log);


    protected void ActivateEditUi() => EditUiRequired = true;

    protected bool EditUiRequired;

    #endregion

    #region App

    /// <inheritdoc />
    public IApp App(NoParamOrder npo = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null)
        => GetApp(ServicesScoped.AppGenerator, npo, zoneId, appId, site, withUnpublished);

    /// <inheritdoc />
    public IApp AppOfSite()
        => GetAndInitApp(ServicesScoped.AppGenerator.New(), GetPrimaryAppIdentity(null), null);

    /// <inheritdoc />
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public IApp AppOfSite(NoParamOrder npo = default, int? siteId = null, ISite? overrideSite = null, bool? withUnpublished = null)
        => GetAndInitApp(ServicesScoped.AppGenerator.New(), GetPrimaryAppIdentity(siteId, overrideSite), overrideSite, withUnpublished);

    #endregion
}