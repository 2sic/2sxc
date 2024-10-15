#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ToSic.Eav.WebApi.Serialization;
using JsonOptions = ToSic.Eav.Serialization.JsonOptions;


namespace ToSic.Sxc.WebApi.ActionFilters;

/// <summary>
/// Make sure .net controllers behave the same in terms of JSON serialization like Newtonsoft
/// </summary>
// https://blogs.taiga.nl/martijn/2020/05/28/system-text-json-and-newtonsoft-json-side-by-side-in-asp-net-core/
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SystemTextJsonFormatterAttribute : ActionFilterAttribute, IControllerModelConvention, IActionModelConvention
{
    public SystemTextJsonFormatterAttribute()
    {
        Order = -3001;
    }

    public void Apply(ControllerModel controller)
    {
        foreach (var action in controller.Actions)
            Apply(action);
    }

    public void Apply(ActionModel action)
    {
        // Set the model binder to NewtonsoftJsonBodyModelBinder for parameters that are bound to the request body.
        var parameters = action.Parameters.Where(p => p.BindingInfo?.BindingSource == BindingSource.Body);
        foreach (var p in parameters)
            p.BindingInfo.BinderType = typeof(SystemTextJsonBodyModelBinder);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            // we need to use System.Text.Json, but generated per request
            // because of DI dependencies for EavJsonConvertors in new generated JsonOptions
            objectResult.Formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(context));

            // Oqtane 3.2.0 and older had NewtonsoftJsonOutputFormatter that we need to remove for our endpoints
            var newtonsoftJsonOutputFormatterType = Type.GetType("NewtonsoftJsonOutputFormatter");
            if (newtonsoftJsonOutputFormatterType != null) objectResult.Formatters.RemoveType(newtonsoftJsonOutputFormatterType);
        }
        else
        {
            base.OnActionExecuted(context);
        }
    }

    private static SystemTextJsonOutputFormatter SystemTextJsonMediaTypeFormatterFactory(ActionExecutedContext context)
    {
        var jsonFormatterAttribute 
            = GetCustomAttributes(((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo).OfType<JsonFormatterAttribute>().FirstOrDefault()
              ?? GetCustomAttributes(context.Controller.GetType()).OfType<JsonFormatterAttribute>().FirstOrDefault();

        // creating JsonConverter, JsonOptions and SystemTextJsonOutputFormatter per request
        // instead of using global, static, singleton version because this is for API only
        var eavJsonConverterFactory = GetEavJsonConverterFactory(jsonFormatterAttribute?.EntityFormat, context);

        var jsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory);

        JsonFormatterHelpers.SetCasing(jsonFormatterAttribute?.Casing ?? Casing.Unspecified, jsonSerializerOptions);

        return new(jsonSerializerOptions);
    }

    private static EavJsonConverterFactory GetEavJsonConverterFactory(EntityFormat? entityFormat, ActionExecutedContext context)
    {
        switch (entityFormat)
        {
            case null:
            case EntityFormat.Light:
                return context.HttpContext.RequestServices.Build<EavJsonConverterFactory>();
            case EntityFormat.None:
            default:
                return null;
        }
    }
}

#endif