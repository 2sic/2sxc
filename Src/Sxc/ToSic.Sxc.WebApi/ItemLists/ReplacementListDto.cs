using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.ItemLists;

public class ReplacementListDto
{
    public int? SelectedId { get; set; }
    public Dictionary<int, string> Items { get; set; }
    public string ContentTypeName { get; set; }
}