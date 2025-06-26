#if NETCOREAPP
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using ToSic.Eav.Serialization.Sys.Json;

namespace ToSic.Sxc.WebApi.Sys.ActionFilters;

public class SystemTextJsonBodyModelBinder(
    ILoggerFactory loggerFactory,
    ArrayPool<char> charPool,
    IHttpRequestStreamReaderFactory readerFactory,
    ObjectPoolProvider objectPoolProvider,
    IOptions<MvcOptions> mvcOptions)
    : BodyModelBinder(GetInputFormatters(loggerFactory, charPool, objectPoolProvider, mvcOptions), readerFactory)
{
    private static IInputFormatter[] GetInputFormatters(
        ILoggerFactory loggerFactory,
        // ReSharper disable UnusedParameter.Local
        ArrayPool<char> charPool,
        ObjectPoolProvider objectPoolProvider,
        IOptions<MvcOptions> mvcOptions)
        // ReSharper restore UnusedParameter.Local
    {
        return
        [
            new SystemTextJsonInputFormatter(SxcJsonOptions, loggerFactory.CreateLogger<SystemTextJsonInputFormatter>())
        ];
    }

    [field: AllowNull, MaybeNull]
    public static Microsoft.AspNetCore.Mvc.JsonOptions SxcJsonOptions
    {
        get
        {
            if (field != null)
                return field;
            field = new();
            field.JsonSerializerOptions.SetUnsafeJsonSerializerOptions();
            return field;
        }
    }
}
#endif