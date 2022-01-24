using System;
using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Services
{
    public class FeaturesService: Wrapper<ToSic.Eav.Configuration.IFeaturesService>, IFeaturesService
    {
        public FeaturesService(ToSic.Eav.Configuration.IFeaturesService contents) : base(contents) { }

        public bool IsEnabled(params string[] nameIds) => _contents.IsEnabled(nameIds);

        public bool Valid => _contents.Valid;

    }
}
