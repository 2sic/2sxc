using System;
using System.Reflection;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Plumbing;

namespace ToSic.Sxc.WebApi.Admin
{
    // TODO: @STV - probably it doesn't need this additional class, just move the code into the ApiExplorerBackend and rename that
    public class ApiExplorerControllerReal<THttpResponseType> : HasLog<ApiExplorerControllerReal<THttpResponseType>>
    {
        public const string LogSuffix = "Explor";

        public ApiExplorerControllerReal(
            Generator<ApiExplorerBackend<THttpResponseType>> backend,
            ResponseMaker<THttpResponseType> responseMaker
            ) : base($"{LogNames.WebApi}.{LogSuffix}Rl")
        {
            _backend = backend;
            _responseMaker = responseMaker;
        }

        private readonly Generator<ApiExplorerBackend<THttpResponseType>> _backend;
        private readonly ResponseMaker<THttpResponseType> _responseMaker;

        public THttpResponseType Inspect(string path, Func<string,Assembly> getAssembly)
        {
            var wrapLog = Log.Call<THttpResponseType>();

            var backend = _backend.New;

            if (backend.PreCheckAndCleanPath(ref path, out var error)) return error;

            try
            {
                return wrapLog(null, backend.AnalyzeClassAndCreateDto(path, getAssembly(path)));
            }
            catch (Exception exc)
            {
                return wrapLog($"Error: {exc.Message}.", _responseMaker.InternalServerError(exc));
            }
        }
    }
}
