using System.Reflection;
using System.Web.Http.Controllers;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

/// <summary>
/// Helper to change the formatters of an API call for this request only.
/// We need it, because every request could have different cultures etc. which is provided through DI.
/// Making it any other way could have side effects on later / other requests.
/// </summary>
internal class PerRequestConfigurationHelper
{

    /// <summary>
    /// Ensure we only configure once per request.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="l"></param>
    /// <returns>`false` if not yet configured, `true` if already configured.</returns>
    internal static bool SkipOnMultipleExecutionsOnTheSameRequest(HttpActionContext context, ILogCall l)
    {
        const string markerKey = "2sxc.JsonFormatter.Configured";

        // Use Request.Properties to mark rather than mutating headers (faster & avoids client-side confusion)
        var props = context.Request?.Properties;
        if (props == null)
            return false;

        // If it was already configured, we would get a count,
        // so we can increase it and log that this is happening - but skip the actual configuration again.
        if (props.TryGetTyped(markerKey, out int cnt))
        {
            cnt++;
            props[markerKey] = cnt;
            l.A($"Formatter configuration already ran for this request - skipping (count:{cnt})");
            return true;
        }

        // First time, mark it as configured and continue with the configuration.
        props[markerKey] = 1;
        return false;
    }

    internal static HttpConfiguration CreateAndApplyPerRequestConfiguration(
        HttpActionContext context,
        DnnJsonFormattersManager manager,
        JsonFormatterAttribute jsonFormatterAttributeOnCurrentAction)
    {
        var configOriginal = context.ControllerContext.Configuration
                             ?? throw new InvalidOperationException("Controller configuration is not available.");

        // Create a copy of the previous configuration
        var settingsCopy = new HttpControllerSettings(configOriginal);


        manager.ReconfigureActionWithContextAwareSerializer(
            context.ControllerContext.ControllerDescriptor,
            settingsCopy.Formatters,    // this list will be updated, side effect!
            jsonFormatterAttributeOnCurrentAction
        );

        var perRequestConfiguration = ApplyControllerSettings(settingsCopy, configOriginal);

        var updated = ApplyPerRequestConfiguration(context, perRequestConfiguration);
        return updated;
    }

    private static HttpConfiguration ApplyPerRequestConfiguration(HttpActionContext context, HttpConfiguration configuration)
    {
        // Note: null should not be possible, will probably cause problems upstream
        if (configuration == null)
            return null;
        context.ControllerContext.Configuration = configuration;
        context.Request?.SetConfiguration(configuration);
        return configuration;
    }

    /// <summary>
    /// This gives access to the internal static method `HttpConfiguration.ApplyControllerSettings`
    /// which is used by Web API to create a controller-specific configuration based on the global configuration and controller settings.
    ///
    /// We need it to spin up per-request clones of the controller configuration without touching shared state
    /// </summary>
    /// <remarks>
    /// We will use this method to create a per-request configuration that we can modify without affecting the global configuration or other requests.
    /// The underlying method is retrieved through reflection, but we'll cache the delegate for performance, so we only pay the reflection cost once.
    ///
    /// The method we wrap is:
    /// internal static HttpConfiguration ApplyControllerSettings(HttpControllerSettings settings, HttpConfiguration configuration)
    /// {
    ///   if (!settings.IsFormatterCollectionInitialized && !settings.IsParameterBindingRuleCollectionInitialized && !settings.IsServiceCollectionInitialized)
    ///     return configuration;
    ///   HttpConfiguration httpConfiguration = new HttpConfiguration(configuration, settings);
    ///   httpConfiguration.Initializer(httpConfiguration);
    ///   return httpConfiguration;
    /// }
    /// </remarks>
    private static readonly Func<HttpControllerSettings, HttpConfiguration, /* out */ HttpConfiguration> ApplyControllerSettings
        = typeof(HttpConfiguration).GetDelegateToMethod<Func<HttpControllerSettings, HttpConfiguration, /* out */ HttpConfiguration>>(
            methodName: "ApplyControllerSettings",
            bindingAttr: BindingFlags.NonPublic | BindingFlags.Static
        );

}