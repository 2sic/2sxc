using System.Collections.Generic;
using ToSic.Sxc.WebApi.SharedDto;
using ToSic.Sxc.WebApi.Usage.Dto;

namespace ToSic.Sxc.WebApi.Context
{
    public class ContentBlockDto : IdentifierDto
    {
        public IEnumerable<InstanceDto> Modules;
    }
}
