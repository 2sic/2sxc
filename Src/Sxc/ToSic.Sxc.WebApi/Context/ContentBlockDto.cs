using System.Collections.Generic;
using ToSic.Sxc.WebApi.SharedDto;

namespace ToSic.Sxc.WebApi.Context
{
    public class ContentBlockDto : IdentifierDto
    {
        public IEnumerable<InstanceDto> Modules;
    }
}
