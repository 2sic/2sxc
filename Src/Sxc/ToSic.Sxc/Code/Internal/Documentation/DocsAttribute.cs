namespace ToSic.Sxc.Code.Internal.Documentation;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DocsAttribute: Attribute
{
    public string[] Messages { get; set; }

    public string[] GetMessages(string fullName)
    {
        if (!AutoLink) return Messages;

        var newMessages = Messages.ToList();
        var helpLink = $"[documentation](https://docs.2sxc.org/api/dot-net/{fullName}.html)";
        newMessages.Add(helpLink);
        return newMessages.ToArray();
    }


    public bool AutoLink = true;

    public bool AllProperties = true;

    public string HelpLink { get; set; }
}