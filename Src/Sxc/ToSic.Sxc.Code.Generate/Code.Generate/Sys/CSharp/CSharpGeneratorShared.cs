using ToSic.Eav.Apps;

namespace ToSic.Sxc.Code.Generate.Sys;

internal class CSharpGeneratorShared
{
    internal static CSharpCodeSpecs BuildSpecs(string appName, IGenerateFileSetSpecs parameters)
    {
        // Core Specs
        var specs = new CSharpCodeSpecs
        {
            AppId = parameters.AppId,
            Edition = parameters.Edition ?? "",
            DateTime = parameters.DateTime,
            Namespace = parameters.Namespace,
            TargetPath = parameters.TargetPath,
            ContentTypes = parameters.ContentTypes,
            Prefix = parameters.Prefix,
            Suffix = parameters.Suffix,
            AppName = appName,
        };

        // Optionally add namespace
        return string.IsNullOrWhiteSpace(parameters.Namespace)
            ? specs
            : specs with { DataNamespace = parameters.Namespace! };
    }
}