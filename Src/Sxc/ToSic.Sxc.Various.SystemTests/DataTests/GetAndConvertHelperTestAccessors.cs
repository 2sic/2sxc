using ToSic.Sxc.Data.Sys.Dynamic;

namespace ToSic.Sxc.DataTests;

internal static class GetAndConvertHelperTestAccessors
{
    public static string[] TacGetFinalLanguagesList(string language, List<string> possibleDims, string[] defaultDims)
        => GetAndConvertHelper.GetFinalLanguagesList(language, possibleDims, defaultDims);
}