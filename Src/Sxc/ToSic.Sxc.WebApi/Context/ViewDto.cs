using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Sxc.WebApi.SharedDto;
using ToSic.Sxc.WebApi.Usage.Dto;

namespace ToSic.Sxc.WebApi.Context
{
    public class ViewDto : IdentifierDto
    {
        public string Name;
        public string Path;
        public IEnumerable<ContentBlockDto> Blocks;

    }
}
