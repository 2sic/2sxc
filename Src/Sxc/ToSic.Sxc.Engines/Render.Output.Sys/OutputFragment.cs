namespace ToSic.Sxc.Render.Output.Sys;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public record OutputFragment
{
    public required string Html { get; init; }
    
    public List<Exception>? ExceptionsOrNull { get; init; }

}