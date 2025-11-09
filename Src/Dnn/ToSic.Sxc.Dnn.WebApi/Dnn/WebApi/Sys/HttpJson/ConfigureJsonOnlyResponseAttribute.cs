using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

/// <summary>
/// Configure the WebApi controller to only use JSON responses.
/// This should enforce JSON formatting.
/// It's only needed in .net4x where the default is xml.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]   // unclear if this needs to be public
public class ConfigureJsonOnlyResponseAttribute : ActionFilterAttribute, IControllerConfiguration
{
    // For debugging
    // ReSharper disable ConvertToConstant.Local
    private static readonly bool TestThrowInitialize = false;
    private static readonly bool TestThrowOnActionExecuting = false;
    // ReSharper restore ConvertToConstant.Local

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

        try
        {
            var formattersManager = new DnnJsonFormattersManager(log);
            formattersManager.ReconfigureControllerWithBestSerializers(
                controllerSettings.Formatters,
                GetCustomAttributes(controllerDescriptor.ControllerType)
            );

            // Test throwing an exception. Usually in debugging you will move the execution cursor to here to generate errors for specific requests
            if (TestThrowInitialize)
                throw new($"Test Exception in {nameof(Initialize)}");

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
        
        try
        {
            var formattersManager = new DnnJsonFormattersManager(log);
            formattersManager.ReconfigureActionWithContextAwareSerializer(
                context.ControllerContext.ControllerDescriptor,
                context.ActionDescriptor
                    .GetCustomAttributes<JsonFormatterAttribute>()
                    .FirstOrDefault()
            );

            // Test throwing an exception. Usually in debugging you will move the execution cursor to here to generate errors for specific requests
            if (TestThrowOnActionExecuting)
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

    
    /// <summary>
    /// since it's really hard to debug attribute/serialization issues, try to log this problem
    /// </summary>
    private static void PlaceLogInHistory(HttpControllerDescriptor controllerDescriptor, ILog log)
    {
        try
        {
            var logStore = controllerDescriptor.Configuration.DependencyResolver.GetService(typeof(ILogStore)) as ILogStore;
            logStore?.Add("webapi-serialization-errors", log);
        }
        catch { /* ignore */ }
    }
}