using System.Collections.Generic;
using ToSic.Sxc.Data.Internal.Dynamic;

namespace ToSic.Sxc.Tests.DataTests;

internal static class GetAndConvertHelperTestAccessors
{
    public static string[] TacGetFinalLanguagesList(string language, List<string> possibleDims, string[] defaultDims)
        => GetAndConvertHelper.GetFinalLanguagesList(language, possibleDims, defaultDims);
}