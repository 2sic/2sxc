using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Dnn.Pages;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class ContentBlockDto: IdentifierBase
    {
        public IEnumerable<ModuleDto> Modules;

        public ContentBlockDto(BlockConfiguration block, IEnumerable<ModuleWithContent> blockModules)
        {
            Id = block.Id;
            Guid = block.Guid;
            Modules = blockModules.Select(m => new ModuleDto(m.Module, m.Page));
        }
    }
}
