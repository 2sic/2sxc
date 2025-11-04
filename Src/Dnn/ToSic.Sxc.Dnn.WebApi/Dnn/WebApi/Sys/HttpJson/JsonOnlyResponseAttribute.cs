using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Eav.WebApi.Sys.Helpers.Json;

// Special case: this should enforce json formatting
// It's only needed in .net4x where the default is xml
namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

[ShowApiWhenReleased(ShowApiMode.Never)]   // unclear if this needs to be public
public class JsonOnlyResponseAttribute : ActionFilterAttribute, IControllerConfiguration
{
    /// <summary>
    /// This will just run once - I think for every controller...
    /// </summary>
    /// <param name="controllerSettings"></param>
    /// <param name="controllerDescriptor"></param>
    public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
    {
        // Create an independent log for this operation - don't use a class-level log because it would grow too much
        var log = new Log("Dnn.JsonAt");
        var l = log.Fn($"{nameof(controllerDescriptor)}:{controllerDescriptor.ControllerType.FullName}");
        var testThrow = false;

        try
        {
            //throw new("test");
            var formatters = controllerSettings.Formatters;

            // Remove the XML formatter
            l.A("Will remove the default XmlFormatter");
            formatters.Remove(formatters.XmlFormatter);

            // Get JsonFormatterAttribute from controller - would mark the controller to use System.Text.Json
            var customAttributes = GetCustomAttributes(controllerDescriptor.ControllerType);
            var jsonFormatterAttribute = customAttributes.OfType<JsonFormatterAttribute>().FirstOrDefault();

            // For older apis we need to leave NewtonsoftJson (when JsonFormatterAttribute is missing on controller)
            var hasRevertToNewtonsoft = customAttributes.OfType<DefaultToNewtonsoftForHttpJsonAttribute>().Any();
            if (hasRevertToNewtonsoft && jsonFormatterAttribute == null)
            {
                l.Done($"Has {nameof(DefaultToNewtonsoftForHttpJsonAttribute)} and no custom {nameof(JsonFormatterAttribute)} will leave serializers intact.");
                return;
            }

            // For newer apis we need to use System.Text.Json, but generated per request
            // because of DI dependencies for EavJsonConvertors in new generated JsonOptions

            // Remove default JsonMediaTypeFormatter (Newtonsoft)
            var newtonSoftFormatters = formatters.OfType<JsonMediaTypeFormatter>().ToList();
            l.A($"Will remove {newtonSoftFormatters.Count} NewtonSoft formatters");
            newtonSoftFormatters.ForEach(f => controllerSettings.Formatters.Remove(f));

            // Add SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
            if (!formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
            {
                l.A($"Will add {nameof(SystemTextJsonMediaTypeFormatter)}");
                formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(controllerDescriptor, jsonFormatterAttribute));
            }
            else
                l.A($"It has a {nameof(SystemTextJsonMediaTypeFormatter)}, so won't add.");

            // Test throwing an exception
            // usually in debugging you will move the execution cursor to here to generate errors for specific requests
            if (testThrow)
                throw new($"Test Exception in {nameof(OnActionExecuting)}");

        }
        catch (Exception ex)
        {
            // Add info - and add to system log as we want to see what actually happened
            l.Ex(ex);
            PlaceLogInHistory(controllerDescriptor, log);
            throw;
        }
    }

    public override void OnActionExecuting(HttpActionContext context)
    {
        var log = new Log("Dnn.JsonAt");
        var l = log.Fn($"{nameof(context.Request.RequestUri)}:{context.Request?.RequestUri}");
        var testThrow = false;
        try
        {
            var controllerDescriptor = context.ControllerContext.ControllerDescriptor;

            // Get JsonFormatterAttribute from action **Method**
            var jsonFormatterAttributeOnAction = context.ActionDescriptor.GetCustomAttributes<JsonFormatterAttribute>().FirstOrDefault();
            if (jsonFormatterAttributeOnAction == null)
            {
                l.A($"{nameof(JsonFormatterAttribute)} is missing on action method.");

                // For older apis we need to leave (when JsonFormatterAttribute is missing on action method)
                var controllerHasDefaultToOld = GetCustomAttributes(controllerDescriptor.ControllerType).OfType<DefaultToNewtonsoftForHttpJsonAttribute>().Any();
                if (controllerHasDefaultToOld)
                {
                    l.Done($"Controller has {nameof(DefaultToNewtonsoftForHttpJsonAttribute)}, will exit leaving old serializers.");
                    return;
                }
            }
            else
                l.A($"Method has custom {nameof(JsonFormatterAttribute)}");

            var formatters = controllerDescriptor.Configuration.Formatters;
            l.A($"Found {formatters.Count} formatters");

            // Remove default SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
            var formattersToRemove = formatters.OfType<SystemTextJsonMediaTypeFormatter>().ToList();
            l.A($"Will remove {formattersToRemove.Count} of type {nameof(SystemTextJsonMediaTypeFormatter)}");
            formattersToRemove.ForEach(f => formatters.Remove(f));

            // Add SystemTextJson JsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from action method
            if (!formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
            {
                l.A($"Will add {nameof(SystemTextJsonMediaTypeFormatter)} since none were found");
                formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(controllerDescriptor, jsonFormatterAttributeOnAction));
            }

            // Test throwing an exception
            // usually in debugging you will move the execution cursor to here to generate errors for specific requests
            if (testThrow)
                throw new($"Test Exception in {nameof(OnActionExecuting)}");
        }
        catch (Exception ex)
        {
            // Add info - and add to system log as we want to see what actually happened
            l.Ex(ex);
            PlaceLogInHistory(context.ControllerContext.ControllerDescriptor, log);
            throw;
        }
    }

    private static SystemTextJsonMediaTypeFormatter SystemTextJsonMediaTypeFormatterFactory(
        HttpControllerDescriptor controllerDescriptor,
        JsonFormatterAttribute jsonFormatterAttribute = null)
    {
        // Get the service provider from the current request scope
        // This ensures all services (including EavJsonConverterFactory and its dependencies) use the current request's culture
        IServiceProvider serviceProvider = null;
        
        // Try to get request-scoped service provider from HttpContext
        var httpContext = HttpContext.Current;
        if (httpContext != null)
        {
            var scope = httpContext.Items[typeof(IServiceScope)] as IServiceScope;
            serviceProvider = scope?.ServiceProvider;
        }
        
        // Fallback to controller's dependency resolver if no scoped provider available
        serviceProvider ??= new DependencyResolverServiceProvider(controllerDescriptor.Configuration.DependencyResolver);

        // Build Eav to Json converters for api v15
        // Important: Get the factory from the request-scoped service provider
        // This ensures it gets fresh IConvertToEavLight with the current request's culture from IZoneCultureResolver
        var eavJsonConverterFactory = GetEavJsonConverterFactory(jsonFormatterAttribute?.EntityFormat, serviceProvider);

        var jsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory);

        JsonFormatterHelpers.SetCasing(jsonFormatterAttribute?.Casing ?? Casing.Unspecified, jsonSerializerOptions);

        return new() { JsonSerializerOptions = jsonSerializerOptions };
    }

    private static EavJsonConverterFactory GetEavJsonConverterFactory(EntityFormat? entityFormat, IServiceProvider serviceProvider)
    {
        switch (entityFormat)
        {
            case null:
            case EntityFormat.Light:
                // Build the factory from the request-scoped service provider
                // This is the key change - it ensures the factory and all its dependencies
                // (including IConvertToEavLight and IZoneCultureResolver) are request-scoped
                return serviceProvider.Build<EavJsonConverterFactory>();
            case EntityFormat.None:
            default:
                return null;
        }
    }


    /// <summary>
    /// since it's really hard to debug attribute/serialization issues, try to log this problem
    /// </summary>
    private static void PlaceLogInHistory(HttpControllerDescriptor controllerDescriptor, ILog log)
    {
        try
        {
            // this variant doesn't work as of DNN 9, not 100% sure why...
            //var logStore = (ILogStore)controllerSettings.Services.GetService(typeof(ILogStore));  // doesn't work
            //var logStore = DnnStaticDi.GetPageScopedServiceProvider().Build<ILogStore>();   // works
            var logStore = controllerDescriptor.Configuration.DependencyResolver.GetService(typeof(ILogStore)) as ILogStore;
            logStore.Add("webapi-serialization-errors", log);
        }
        catch { /* ignore */ }
    }

    /// <summary>
    /// Helper class to wrap DependencyResolver as IServiceProvider for compatibility
    /// </summary>
    private class DependencyResolverServiceProvider(IDependencyResolver resolver) : IServiceProvider
    {
        public object GetService(Type serviceType) => resolver.GetService(serviceType);
    }
}