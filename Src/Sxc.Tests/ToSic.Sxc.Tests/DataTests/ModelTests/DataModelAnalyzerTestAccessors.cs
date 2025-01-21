using System;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

internal static class DataModelAnalyzerTestAccessors
{
    public static string GetContentTypeNameTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetContentTypeNames<T>();

    public static string GetStreamNameTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetStreamName<T>();

    public static Type GetTargetTypeTac<T>()
        where T : class, ICanWrapData
        => DataModelAnalyzer.GetTargetType<T>();
}