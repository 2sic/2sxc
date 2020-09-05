using System.Collections.Generic;
using ToSic.Sxc.WebApi.Usage.Dto;

namespace ToSic.Sxc.WebApi.Context
{
    public class ContentBlockDto : IdentifierBase
    {
        public IEnumerable<InstanceDto> Modules;
    }
}
