namespace ToSic.Sxc.Engines;

/// <summary>
/// Attribute to mark all IEngine implementations
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EngineDefinitionAttribute: Attribute
{
    public EngineDefinitionAttribute() { }

    public string Name { get; set; } = Eav.Constants.NullNameId;


}