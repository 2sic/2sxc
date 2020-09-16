using System;

namespace ToSic.Sxc.WebApi.Views
{
    public class ViewDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public object ContentType { get; set; }
        public object PresentationType { get; set; }
        public object ListContentType { get; set; }
        public object ListPresentationType { get; set; }
        public string TemplatePath { get; set; }
        public bool IsHidden { get; set; }
        public string ViewNameInUrl { get; set; }
        public Guid Guid { get; set; }
        public bool List { get; set; }
        public bool HasQuery { get; set; }
        public int Used { get; set; }
    }
}
