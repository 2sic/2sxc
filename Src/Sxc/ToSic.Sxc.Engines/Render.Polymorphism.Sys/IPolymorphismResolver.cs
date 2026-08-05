namespace ToSic.Sxc.Render.Polymorphism.Sys;

/// <summary>
/// A polymorphism resolver - which can determine alternate editions for a view / template
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IPolymorphismResolver: ISwitchableService
{
    string? Edition(PolymorphismConfigurationModel config, string? overrule, ILog log);
}