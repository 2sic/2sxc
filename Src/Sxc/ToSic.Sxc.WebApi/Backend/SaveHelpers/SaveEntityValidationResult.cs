using ToSic.Eav.WebApi.Sys.Helpers.Http;

namespace ToSic.Sxc.Backend.SaveHelpers;

[PrivateApi]
public record SaveEntityValidationResult(HttpExceptionAbstraction? Exception = null)
{
    public bool IsValid => Exception == null;
}