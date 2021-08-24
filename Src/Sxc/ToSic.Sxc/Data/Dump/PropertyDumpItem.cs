using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Data.Dump
{
    [PrivateApi]
    public class PropertyDumpItem
    {
        public string Source { get; set; }
        public string Path { get; set; }
        public object Value { get; set; }
    }
}
