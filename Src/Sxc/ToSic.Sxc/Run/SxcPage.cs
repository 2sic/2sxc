using System.Collections.Generic;
using ToSic.Eav.Apps.Run;

namespace ToSic.Sxc.Run
{
    public class SxcPage: IPage
    {
        public SxcPage(int id, string url, List<KeyValuePair<string, string>> parameters)
        {
            Id = id;
            Parameters = parameters;
            Url = url ?? "url-unknown";
        }

        public int Id { get; }
        public List<KeyValuePair<string, string>> Parameters { get; }
        public string Url { get; }
    }
}
