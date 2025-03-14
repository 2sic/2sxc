using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.DataTests.ModelTests;

internal static class DataModelAnalyzerTestAccessors
{
    //public static string GetContentTypeNameTac<T>()
    //    where T : class, ICanWrapData
        //=> DataModelAnalyzer.GetContentTypeNameCsv<T>();
    public static (List<string> List, string Flat) GetContentTypeNamesTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetContentTypeNamesList<T>();

    public static (List<string> List, string Flat) GetStreamNameListTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetStreamNameList<T>();

    public static Type GetTargetTypeTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetTargetType<T>();
}