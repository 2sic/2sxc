using System.Collections.Generic;
using ToSic.Sxc.WebApi.SharedDto;

namespace ToSic.Sxc.WebApi.Context
{
    public class ViewDto : IdentifierDto
    {
        public string Name;
        public string Path;
        public IEnumerable<ContentBlockDto> Blocks;

    }
}
