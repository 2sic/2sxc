using System;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Models;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

internal static class DataModelAnalyzerTestAccessors
{
    public static string GetContentTypeNameTac<T>()
        where T : class, IDataModel
        => DataModelAnalyzer.GetContentTypeNames<T>();

    public static string GetStreamNameTac<T>()
        where T : class, IDataModel
        => DataModelAnalyzer.GetStreamName<T>();

    public static Type GetTargetTypeTac<T>()
        where T : class, IDataModel
        => DataModelAnalyzer.GetTargetType<T>();
}