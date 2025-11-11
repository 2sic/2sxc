using System.Reflection;
using System.Web.Http.Controllers;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

internal class PerRequestConfigurationHelper
{
    private const string MarkerKey = "2sxc.JsonFormatter.Configured";

    /// <summary>
    /// Ensure we only configure once per request
    /// </summary>
    /// <param name="context"></param>
    /// <param name="l"></param>
    /// <returns></returns>
    internal static bool SkipOnMultipleExecutionsOnTheSameRequest(HttpActionContext context, ILogCall l)
    {
        // Use Request.Properties to mark rather than mutating headers (faster & avoids client-side confusion)
        var props = context.Request?.Properties;
        if (props != null)
        {
            if (props.TryGetTyped(MarkerKey, out int cnt))
            {
                cnt++;
                props[MarkerKey] = cnt;
                l.A($"Formatter configuration already ran for this request - skipping (count:{cnt})");
                return true;
            }
            props[MarkerKey] = 1; // first time
        }
        return false;
    }

    internal static HttpConfiguration CreatePerRequestConfiguration(
        HttpActionContext context,
        DnnJsonFormattersManager manager,
        JsonFormatterAttribute jsonFormatterAttributeOnAction)
    {
        var originalConfig = context.ControllerContext.Configuration
                             ?? throw new InvalidOperationException("Controller configuration is not available.");

        var controllerSettings = new HttpControllerSettings(originalConfig);

        manager.ReconfigureActionWithContextAwareSerializer(
            context.ControllerContext.ControllerDescriptor,
            controllerSettings.Formatters,
            jsonFormatterAttributeOnAction);

        return ApplyControllerSettingsAccessor.Value(controllerSettings, originalConfig);
    }

    internal static void ApplyPerRequestConfiguration(HttpActionContext context, HttpConfiguration configuration)
    {
        if (configuration == null)
            return;
        context.ControllerContext.Configuration = configuration;
        context.Request?.SetConfiguration(configuration);
    }

    // Cached reflection bridge to Web API’s internal HttpConfiguration.ApplyControllerSettings
    // so we can spin up per-request clones of the controller configuration without touching shared state 
    private static readonly Lazy<Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>> ApplyControllerSettingsAccessor
        = new(() =>
        {
            var method = typeof(HttpConfiguration)
                .GetMethod("ApplyControllerSettings", BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
                throw new InvalidOperationException("Unable to locate HttpConfiguration.ApplyControllerSettings");
            return (Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>)method
                .CreateDelegate(typeof(Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>));
        });
}