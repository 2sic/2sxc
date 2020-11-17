using System.IO;
using Newtonsoft.Json;

namespace ToSic.Sxc.WebApi.Assets
{
    public class FileUploadDto
    {
        public string Name;
        public Stream Stream;

        [JsonIgnore]
        public string Contents 
        {
            get
            {
                if (_contents != null) return _contents;
                using (var fileStreamReader = new StreamReader(Stream)) _contents = fileStreamReader.ReadToEnd();
                return _contents;
            }
        }

        private string _contents;
    }
}
