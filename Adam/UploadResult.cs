namespace ToSic.SexyContent.Adam
{
    public class UploadResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Filename { get; set; }
        public int FileId { get; set; }
        public string FullPath { get; set; }
    }
}