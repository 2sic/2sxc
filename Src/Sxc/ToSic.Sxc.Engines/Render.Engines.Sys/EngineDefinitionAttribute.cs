using ToSic.Eav.Sys;

namespace ToSic.Sxc.Render.Engines.Sys;

/// <summary>
/// Attribute to mark all IEngine implementations - for future use, when more generic engines exist.
/// At the moment, it doesn't really seem to serve a purpose.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineDefinitionAttribute: Attribute
{
    public string Name { get; init; } = EavConstants.NullNameId;
}