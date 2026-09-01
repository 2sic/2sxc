using ToSic.Eav.Models;

namespace ToSic.Sxc.Render.Polymorphism.Sys;

/// <summary>
/// Model to read an Apps Polymorphism configuration.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ModelSpecs(ContentType = ContentTypeNameId)]
public record PolymorphismConfigurationModel : ModelFromEntityFull
{
    private const string ContentTypeNameId = "3937fa17-ef2d-40a7-b089-64164eb10bab";
    [PrivateApi]
    public const string ContentTypeName = "2sxcPolymorphismConfiguration";

    /// <summary>
    /// Polymorphism mode, atm 
    /// </summary>
    public string Mode => GetThis("");

    public string UsersWhoMaySwitchEditions => GetThis("");

    [field: AllowNull, MaybeNull]
    public List<int> UsersWhoMaySwitch => field ??= new Func<List<int>>(() => UsersWhoMaySwitchEditions
        .CsvToArrayWithoutEmpty()
        .Select(s => int.TryParse(s, out var result) ? result : -1)
        .Where(i => i > 0)
        .ToList()
    )();

    /// <summary>
    /// Name of the resolver to use, like `koi` or `permissions`.
    /// </summary>
    /// <remarks>
    /// It could be any value, if the DI has such as named service registered.
    /// </remarks>
    public string? Resolver => SplitMode().Resolver;

    /// <summary>
    /// Additional parameter behind the resolver type, like `cssFramework` for the Koi resolver.
    /// </summary>
    public string? Parameters => SplitMode().Parameters;

    private (string? Resolver, string? Parameters) SplitMode()
    {
        if (_resolverAndParameters != default)
            return _resolverAndParameters;

        var rule = Mode;
        if (string.IsNullOrEmpty(Mode))
            return (null, null);
        var parts = rule.Split('?');
        var resolver = parts[0];
        var parameters = parts.Length > 0 ? parts[1] : null;
        return _resolverAndParameters = (resolver, parameters);
    }
    private (string? Resolver, string? Parameters) _resolverAndParameters;
}