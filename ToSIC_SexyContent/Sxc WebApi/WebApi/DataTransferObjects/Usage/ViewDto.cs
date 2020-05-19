using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Pages;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class ViewDto
    {
        public int Id;
        public Guid Guid;
        public string Name;
        public string Path;
        public IEnumerable<ContentBlockDto> Blocks;

        public ViewDto(IView view, List<BlockConfiguration> blocks, List<ModuleWithContent> modules)
        {
            Id = view.Entity.EntityId;
            Guid = view.Entity.EntityGuid;
            Name = view.Name;
            Path = view.Path;
            Blocks = blocks
                .Where(b => b.View.Guid == view.Guid)
                .Select(blWMod => new ContentBlockDto(blWMod,
                    modules.Where(m => m.ContentGroup == blWMod.Guid)));
        }
    }
}
