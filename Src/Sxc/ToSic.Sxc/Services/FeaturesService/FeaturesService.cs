using System;
using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Services
{
    public class FeaturesService: Wrapper<ToSic.Eav.Configuration.IFeaturesService>, IFeaturesService
    {
        public FeaturesService(ToSic.Eav.Configuration.IFeaturesService contents) : base(contents) { }

        public bool Enabled(Guid guid) => _contents.Enabled(guid);

        public bool Enabled(string nameId) => _contents.Enabled(nameId);

        public bool Enabled(IEnumerable<Guid> guids) => _contents.Enabled(guids);

        public bool Enabled(IEnumerable<string> nameIds) => _contents.Enabled(nameIds);

        public bool Valid => _contents.Valid;

    }
}
