using System.Runtime.CompilerServices;
using System;
using ToSic.Sxc.Services;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Data;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static class CodeDataFactoryExtensions
{
    /// <summary>
    /// Will check if the CodeDataFactory exists and try to get the ServiceKit - or throw an error. 
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    internal static ServiceKit16 GetServiceKitOrThrow(this CodeDataFactory cdf, [CallerMemberName] string cName = default)
    {
        if (cdf == null)
            throw new NotSupportedException(
                $"Trying to use {cName}(...) in a scenario where the {nameof(cdf)} is not available.");

        var kit = cdf._DynCodeRoot.GetKit<ServiceKit16>();
        return kit ?? throw new NotSupportedException(
            $"Trying to use {cName}(...) in a scenario where the {nameof(ServiceKit16)} is not available.");
    }

}