using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        /// <inheritdoc />
        [PrivateApi("WIP")]
        public dynamic Resources => _resources ?? (_resources = new DynamicStack(
                new DynamicEntityDependencies(Block,
                    DataSourceFactory.ServiceProvider,
                    CmsContext.SafeLanguagePriorityCodes()),
                new KeyValuePair<string, IPropertyLookup>("View", Block?.View?.Resources),
                new KeyValuePair<string, IPropertyLookup>("App", App?.Resources?.Entity))
            );
        private dynamic _resources;

        /// <inheritdoc />
        [PrivateApi("WIP")]
        public dynamic Settings => _settings ?? (_settings = new DynamicStack(
                new DynamicEntityDependencies(_DynCodeRoot.Block,
                    _DynCodeRoot.DataSourceFactory.ServiceProvider,
                    CmsContext.SafeLanguagePriorityCodes()),
                new KeyValuePair<string, IPropertyLookup>("View", _DynCodeRoot.Block?.View?.Settings),
                new KeyValuePair<string, IPropertyLookup>("App", _DynCodeRoot.App?.Settings?.Entity))
            );
        private dynamic _settings;
    }
}
