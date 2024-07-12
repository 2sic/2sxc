#if NETCOREAPP
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.WebApi.ActionFilters;

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
        ArrayPool<char> charPool,
        ObjectPoolProvider objectPoolProvider,
        IOptions<MvcOptions> mvcOptions)
    {
        return
        [
            new SystemTextJsonInputFormatter(SxcJsonOptions, loggerFactory.CreateLogger<SystemTextJsonInputFormatter>())
        ];
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
#endif