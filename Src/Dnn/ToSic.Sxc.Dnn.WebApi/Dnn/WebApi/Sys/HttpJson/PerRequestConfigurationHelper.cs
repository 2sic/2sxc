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
    /// Ensure we only configure once per request
    /// </summary>
    /// <param name="context"></param>
    /// <param name="l"></param>
    /// <returns></returns>
    internal static bool SkipOnMultipleExecutionsOnTheSameRequest(HttpActionContext context, ILogCall l)
    {
        const string markerKey = "2sxc.JsonFormatter.Configured";

        // Use Request.Properties to mark rather than mutating headers (faster & avoids client-side confusion)
        var props = context.Request?.Properties;
        if (props != null)
        {
            if (props.TryGetTyped(markerKey, out int cnt))
            {
                cnt++;
                props[markerKey] = cnt;
                l.A($"Formatter configuration already ran for this request - skipping (count:{cnt})");
                return true;
            }
            props[markerKey] = 1; // first time
        }
        return false;
    }

    internal static HttpConfiguration CreateAndApplyPerRequestConfiguration(
        HttpActionContext context,
        DnnJsonFormattersManager manager,
        JsonFormatterAttribute jsonFormatterAttributeOnAction)
    {
        var configOriginal = context.ControllerContext.Configuration
                             ?? throw new InvalidOperationException("Controller configuration is not available.");

        // Create a copy of the previous configuration
        var settingsCopy = new HttpControllerSettings(configOriginal);

        manager.ReconfigureActionWithContextAwareSerializer(
            context.ControllerContext.ControllerDescriptor,
            settingsCopy.Formatters,    // this list will be updated, side effect!
            jsonFormatterAttributeOnAction
        );

        var perRequestConfiguration = ApplyControllerSettings(settingsCopy, configOriginal);

        var updated = ApplyPerRequestConfiguration(context, perRequestConfiguration);
        return updated;
    }

    private static HttpConfiguration ApplyPerRequestConfiguration(HttpActionContext context, HttpConfiguration configuration)
    {
        if (configuration == null)
            return null;
        context.ControllerContext.Configuration = configuration;
        context.Request?.SetConfiguration(configuration);
        return configuration;
    }

    // Cached reflection bridge to Web API’s internal HttpConfiguration.ApplyControllerSettings
    // so we can spin up per-request clones of the controller configuration without touching shared state 
    private static readonly Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration> ApplyControllerSettings
        = GetInternalApplyControllerSettings();

    private static Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration> GetInternalApplyControllerSettings()
    {
        const string methodName = "ApplyControllerSettings";
        var method = typeof(HttpConfiguration).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
            throw new InvalidOperationException($"Unable to locate {nameof(HttpConfiguration)}.{methodName}");
        return (Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>)method
            .CreateDelegate(typeof(Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>));


        // Note: the method is this: 
        //internal static HttpConfiguration ApplyControllerSettings(
        //    HttpControllerSettings settings,
        //    HttpConfiguration configuration)
        //{
        //    if (!settings.IsFormatterCollectionInitialized && !settings.IsParameterBindingRuleCollectionInitialized && !settings.IsServiceCollectionInitialized)
        //        return configuration;
        //    HttpConfiguration httpConfiguration = new HttpConfiguration(configuration, settings);
        //    httpConfiguration.Initializer(httpConfiguration);
        //    return httpConfiguration;
        //}
    }
}