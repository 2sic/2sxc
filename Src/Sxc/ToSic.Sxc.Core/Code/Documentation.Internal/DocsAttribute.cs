namespace ToSic.Sxc.Code.Internal.Documentation;

/// <summary>
/// This was an experimental way to add attributes to classes, which should generate documentation for the
/// VS Code Editor in the browser.
///
/// It has been dormant for many years now, and will probably be removed since we now include important XML files in the bin,
/// and should probably retrieve the docs from there if we continue with this.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DocsAttribute: Attribute
{
    public required string[] Messages { get; set; }

    public string[] GetMessages(string? fullName)
    {
        if (fullName == null)
            return [];

        if (!AutoLink)
            return Messages;

        var newMessages = Messages.ToList();
        var helpLink = $"[documentation](https://docs.2sxc.org/api/dot-net/{fullName}.html)";
        newMessages.Add(helpLink);
        return newMessages.ToArray();
    }


    public bool AutoLink = true;

    public bool AllProperties = true;

    public string? HelpLink { get; set; }
}