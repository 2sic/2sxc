using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Models.Sys;

namespace ToSic.Sxc.DataTests.ModelTests;

internal static class DataModelAnalyzerTestAccessors
{
    public static List<string> GetContentTypeNamesTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetContentTypeNamesList<T>();

    public static List<string> GetStreamNameListTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetStreamNameList<T>();

    public static Type GetTargetTypeTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetTargetType<T>();
}