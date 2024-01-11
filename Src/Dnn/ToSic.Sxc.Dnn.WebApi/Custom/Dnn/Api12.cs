using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Internal.HttpJson;

// ReSharper disable once CheckNamespace
namespace Custom.Dnn;

/// <summary>
/// This is the base class for all custom API Controllers. <br/>
/// With this, your code receives the full context  incl. the current App, DNN, Data, etc.
/// </summary>
[PublicApi("This is the official base class for v12+")]
[DnnLogExceptions]
[DefaultToNewtonsoftForHttpJson]
public abstract class Api12(string logSuffix) : Hybrid.Api12(logSuffix), IDnnDynamicWebApi, IHasDnn
{
    protected Api12() : this("Dnn12") { }

    /// <inheritdoc cref="IHasDnn.Dnn"/>
    public IDnnContext Dnn => (_CodeApiSvc as IHasDnn)?.Dnn;
}