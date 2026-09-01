using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.WebApi.Tests.CodeGeneration;

internal sealed record TestFileGeneratorSpecs : IFileGeneratorSpecs
{
    public string? Configuration { get; init; }
    public int AppId { get; init; }
    public string? Edition { get; init; }
    public DateTime DateTime { get; init; } = DateTime.Now;
    public string? Namespace { get; init; }
    public string? TargetPath { get; init; }
    public ICollection<string>? ContentTypes { get; init; }
    public string? Prefix { get; init; }
    public string? Suffix { get; init; }
}

