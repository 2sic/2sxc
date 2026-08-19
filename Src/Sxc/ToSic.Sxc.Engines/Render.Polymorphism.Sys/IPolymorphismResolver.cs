namespace ToSic.Sxc.Render.Polymorphism.Sys;

/// <summary>
/// A polymorphism resolver - which can determine alternate editions for a view / template
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IPolymorphismResolver
{
    /// <summary>
    /// Returns the edition to use for a given polymorphism configuration, or null if no edition is found
    /// </summary>
    /// <param name="config">Polymorphism configuration.</param>
    /// <param name="overrule">Optional overrule value; may not be respected, in case the resolver determines insufficient permissions to overrule.</param>
    /// <param name="log">Logger is included, so that the services can be super-slim.</param>
    /// <returns>The edition to use, or null if no edition is found</returns>
    string? Edition(PolymorphismConfigurationModel config, string? overrule, ILog log);
}