using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Linq;
using ToSic.Eav.Serialization;
using ToSic.Eav.WebApi.Serialization;
using ToSic.Lib.DI;
using ToSic.Sxc.WebApi;
using JsonOptions = ToSic.Eav.Serialization.JsonOptions;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    // https://blogs.taiga.nl/martijn/2020/05/28/system-text-json-and-newtonsoft-json-side-by-side-in-asp-net-core/
    public class SystemTestJsonFormatterAttribute : ActionFilterAttribute, IControllerModelConvention, IActionModelConvention
    {
        public SystemTestJsonFormatterAttribute()
        {
            Order = -3001;
        }

        public void Apply(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                Apply(action);
            }
        }

        public void Apply(ActionModel action)
        {
            // Set the model binder to NewtonsoftJsonBodyModelBinder for parameters that are bound to the request body.
            var parameters = action.Parameters.Where(p => p.BindingInfo?.BindingSource == BindingSource.Body);
            foreach (var p in parameters)
            {
                p.BindingInfo.BinderType = typeof(SystemTextJsonBodyModelBinder);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var jsonFormatterAttribute = GetCustomAttributes(context.Controller.GetType()).OfType<JsonFormatterAttribute>().FirstOrDefault();

                // creating JsonConverter, JsonOptions and SystemTextJsonOutputFormatter per request
                // instead of using global, static, singleton version because this is for API only
                var eavJsonConverterFactory = (jsonFormatterAttribute?.AutoConvertEntity == false) ? null : context.HttpContext.RequestServices.Build<EavJsonConverterFactory>();
                var jsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory);
                var systemTextJsonOutputFormatter = new SystemTextJsonOutputFormatter(jsonSerializerOptions);
                if (jsonFormatterAttribute?.Casing == Casing.CamelCase)
                    jsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                objectResult.Formatters.Add(systemTextJsonOutputFormatter);

                // Oqtane 3.2.0 and older had NewtonsoftJsonOutputFormatter that we need to remove for our endpoints
                var newtonsoftJsonOutputFormatterType = Type.GetType("NewtonsoftJsonOutputFormatter");
                if (newtonsoftJsonOutputFormatterType != null) objectResult.Formatters.RemoveType(newtonsoftJsonOutputFormatterType);
            }
            else
            {
                base.OnActionExecuted(context);
            }
        }
    }

    public class SystemTextJsonBodyModelBinder : BodyModelBinder
    {
        public SystemTextJsonBodyModelBinder(
            ILoggerFactory loggerFactory,
            ArrayPool<char> charPool,
            IHttpRequestStreamReaderFactory readerFactory,
            ObjectPoolProvider objectPoolProvider,
            IOptions<MvcOptions> mvcOptions)
            : base(GetInputFormatters(loggerFactory, charPool, objectPoolProvider, mvcOptions), readerFactory)
        { }

        private static IInputFormatter[] GetInputFormatters(
            ILoggerFactory loggerFactory,
            ArrayPool<char> charPool,
            ObjectPoolProvider objectPoolProvider,
            IOptions<MvcOptions> mvcOptions)
        {
            return new IInputFormatter[]
            {
                new SystemTextJsonInputFormatter(SxcJsonOptions, loggerFactory.CreateLogger<SystemTextJsonInputFormatter>())
            };
        }

        public static Microsoft.AspNetCore.Mvc.JsonOptions SxcJsonOptions
        {
            get
            {
                if (_sxcJsonOptions == null)
                {
                    _sxcJsonOptions = new();
                    _sxcJsonOptions.JsonSerializerOptions.SetUnsafeJsonSerializerOptions();
                }
                return _sxcJsonOptions;
            }
        }
        private static Microsoft.AspNetCore.Mvc.JsonOptions _sxcJsonOptions;

    }
}
