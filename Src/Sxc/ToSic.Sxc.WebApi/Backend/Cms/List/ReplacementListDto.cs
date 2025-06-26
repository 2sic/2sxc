namespace ToSic.Sxc.Backend.Cms;

public class ReplacementListDto
{
    public int? SelectedId { get; set; }
    public required Dictionary<int, string> Items { get; set; }
    public required string ContentTypeName { get; set; }
}