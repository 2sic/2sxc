namespace ToSic.Sxc.WebApi;

/// <summary>
/// Determines what casing to use when converting data to JSON.
/// This is for the <see cref="JsonFormatterAttribute"/>.
/// Can be used as flags, so you can say `Casing = Casing.CamelCase` or `Casing = Casing.ObjectPascal | Casing.DictionaryCamel`
/// </summary>
[PublicApi]
[Flags]
public enum Casing
{
    /// <summary>
    /// No casing configuration set.
    /// Will preserve casing as it is, usually Pascal case (old behavior for 2sxc Apis).
    /// </summary>
    [PrivateApi("Hidden for now, as it doesn't matter to external users")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    Unspecified = 0,

    /// <summary>
    /// Set casing to use camelCase for everything.
    /// This is how most JavaScript code expects the data.
    /// The opposite would be <see cref="Preserve"/>.
    /// </summary>
    Camel = 1 << 0,

    /// <summary>
    /// Set casing to use original name for everything - usually PascalCase as is common in C#.
    /// This is how conversion would have worked before v15, as the C# objects all use CamelCase internally.
    /// The opposite would be <see cref="Camel"/>
    /// </summary>
    Preserve = 1 << 2,

    /// <summary>
    /// Set casing of Dictionaries to be camelCase.
    /// For example, Entity properties such as `Birthday` = `birthday`, `FirstName` = `firstName`.
    /// This would be Camel case.
    /// </summary>
    DictionaryCamel = 1 << 9,

    /// <summary>
    /// Set casing of Dictionaries to be PascalCase.
    /// For example, Entity properties such as `Birthday` = `Birthday`, `FirstName` = `firstName`.
    /// This would be Camel case.
    /// </summary>
    DictionaryPreserve = 1 << 10,
}