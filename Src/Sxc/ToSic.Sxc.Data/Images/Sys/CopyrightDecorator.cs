namespace ToSic.Sxc.Images.Sys;

// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
[ModelSpecs(ContentType = ContentTypeNameId)]
public record CopyrightDecorator : ModelOfEntityCore
{
    public const string ContentTypeNameId = "077835ec-889e-433f-8acf-a4715acb3503";
    public const string ContentTypeName = "CopyrightDecorator";

    /// <summary>
    /// Specifies what kind of information is used. `` (blank), `simple` (text only),`advanced` - selecting the data from Copyright entities.
    /// </summary>
    public string? CopyrightInfoType => GetThis<string>(null);

    /// <summary>
    /// Simple Copyright Message such as "© 2027 My Company".
    /// </summary>
    public string? CopyrightMessage => GetThis<string>(null);

    // TODO: THIS will probably not work yet
    /// <summary>
    /// Copyright Entities of type <see cref="CopyrightPreset"/>.
    /// </summary>
    public IEnumerable<IEntity>? Copyrights => GetThis<object>(null) as IEnumerable<IEntity>;
}