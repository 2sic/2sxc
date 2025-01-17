using System;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Model;
using ToSic.Sxc.Models;

namespace ToSic.Sxc.Tests.DataTests.ModelTests;

internal static class DataModelAnalyzerTestAccessors
{
    public static string GetContentTypeNameTac<T>()
        where T : class, IDataWrapper
        => DataModelAnalyzer.GetContentTypeNames<T>();

    public static string GetStreamNameTac<T>()
        where T : class, IDataWrapper
        => DataModelAnalyzer.GetStreamName<T>();

    public static Type GetTargetTypeTac<T>()
        where T : class, IDataWrapper
        => DataModelAnalyzer.GetTargetType<T>();
}