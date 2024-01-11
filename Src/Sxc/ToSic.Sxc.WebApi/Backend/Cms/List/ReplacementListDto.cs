namespace ToSic.Sxc.Backend.Cms;

public class ReplacementListDto
{
    public int? SelectedId { get; set; }
    public Dictionary<int, string> Items { get; set; }
    public string ContentTypeName { get; set; }
}