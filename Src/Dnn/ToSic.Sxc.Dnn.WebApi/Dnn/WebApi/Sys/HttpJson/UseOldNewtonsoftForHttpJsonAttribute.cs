using System.Net.Http.Formatting;
using System.Web.Http.Controllers;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

/// <summary>
/// Mark all base classes for custom WebApi controllers which should use the old Newtonsoft.
/// Important because it otherwise breaks existing code which may be using Newtonsoft objects - specifically old Mobius WebApi controllers.
/// https://github.com/2sic/2sxc/issues/2917
/// Should only be applied to the base classes up to Api14, but not on newer classes
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]   // unclear if this needs to be public
public class DefaultToNewtonsoftForHttpJsonAttribute : Attribute
{
    /// <summary>
    /// For older apis we need to leave NewtonsoftJson as the serializer.
    /// </summary>
    /// <param name="controllerSettings"></param>
    /// <param name="controllerDescriptor"></param>
    public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        => SetDefaultNewtonsoftJsonFormatter(controllerSettings);

    private void SetDefaultNewtonsoftJsonFormatter(HttpControllerSettings controllerSettings)
    {
        // Remove System.Text.Json JsonMediaTypeFormatter
        var formatters = controllerSettings.Formatters
            .OfType<SystemTextJsonMediaTypeFormatter>()
            .ToList();
        foreach (var f in formatters)
            controllerSettings.Formatters.Remove(f);

        // Bring back original JsonFormatter
        if (!controllerSettings.Formatters.OfType<JsonMediaTypeFormatter>().Any())
            controllerSettings.Formatters.Insert(0, controllerSettings.Formatters.JsonFormatter);
    }
}