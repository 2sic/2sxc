namespace ToSic.Sxc.Backend.Admin
{
    /// <summary>
    /// Used to deserialize 'editions' from app.json
    /// </summary>
    public class EditionsJson
    {
        public bool IsConfigured { get; set; }
        public Dictionary<string, EditionInfo> Editions { get; set; } = [];
    }

    public class EditionInfo(string description)
    {
        public string Description { get; set; } = description;
    }

    public static class EditionsJsonExtension
    {
        public static EditionsDto ToEditionsDto(this EditionsJson editionsJson)
            => new()
            {
                IsConfigured = true,
                Editions = editionsJson.Editions.Select(e => new EditionDto() { Name = e.Key, Description = e.Value.Description }).ToList()
            };
    }
}
