namespace ToSic.Sxc.WebApi.Adam
{
    public class UploadResultDto
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string Path { get; set; }

        public string Type { get; set; }
    }
}