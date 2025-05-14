using System.Runtime.CompilerServices;
using ToSic.Lib.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data.Internal;

internal static class CodeDataFactoryExtensions
{
    /// <summary>
    /// Will check if the CodeDataFactory exists and try to get the ServiceKit - or throw an error. 
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    internal static IServiceKitForTypedData GetServiceKitOrThrow(this ICodeDataFactory cdf, [CallerMemberName] string cName = default)
    {
        if (cdf == null)
            throw new NotSupportedException($"Trying to use {cName}(...) in a scenario where the {nameof(cdf)} is not available.");

        var kit = ((IWrapper<IServiceKitForTypedData>)cdf._CodeApiSvc).GetContents(); // before v20 it was .GetKit<ServiceKit16>();
        return kit ?? throw new NotSupportedException(
            $"Trying to use {cName}(...) in a scenario where the {nameof(IServiceKitForTypedData)} is not available.");
    }

}