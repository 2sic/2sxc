namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// Temporary object to hold the generated code and usings,
/// so that the caller can later merge all required usings.
/// </summary>
/// <param name="code"></param>
/// <param name="usings"></param>
/// <param name="closing">special closing string if the code wraps something else</param>
internal class CodeFragment(string nameId, string code, bool priority = true, List<string> usings = default, string closing = default)
{
    /// <summary>
    /// Distinct property name to deduplicate, in case multiple fragments would generate the same property.
    ///
    /// like if we had a string property `ImageFile` and a file property `Image` which would both generate a property `ImageFile`,
    /// </summary>
    public string NameId { get; } = nameId;

    /// <summary>
    /// Priority to respect when deduplicating properties.
    /// </summary>
    public bool Priority { get; } = priority;

    /// <summary>
    /// List of additional usings when generating, which would end up on top.
    /// </summary>
    public List<string> Usings { get; } = usings ?? [];

    /// <summary>
    /// ToString should basically create the code - without contents.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToString(null);

    /// <summary>
    /// Convert to string, wrapping the contents with the code and closing.
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    public string ToString(string contents) => code + contents + closing;
}