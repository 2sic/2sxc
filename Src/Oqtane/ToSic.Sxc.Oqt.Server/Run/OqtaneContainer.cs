using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Web;

// TODO: #Oqtane - still very random - uses lots of TestIds

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneContainer: IContainer
    {
        public OqtaneContainer(int? tenantId = null, int? id = null, int? appId = null, Guid? block = null)
        {
            TenantId = tenantId ?? TestIds.Blog.Zone;
            Id = id ?? TestIds.Blog.Container;
            AppId = appId ?? TestIds.Blog.App;
            Block = block ?? TestIds.Blog.Block;
        }

        // Temp implementation, don't support im MVC
        public IContainer Init(int id, ILog parentLog) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public int TenantId { get; }

        /// <inheritdoc />
        public bool IsPrimary => BlockIdentifier.AppId == TestIds.PrimaryApp;

        public List<KeyValuePair<string, string>> Parameters
        {
            get => _parameters ??
                   (_parameters = Eav.Factory.Resolve<IHttp>().QueryStringKeyValuePairs());
            set => _parameters = value;
        }
        private List<KeyValuePair<string, string>> _parameters;

        public IBlockIdentifier BlockIdentifier 
            => _blockIdentifier ?? (_blockIdentifier = new BlockIdentifier(TenantId, AppId, Block, Guid.Empty));

        private IBlockIdentifier _blockIdentifier;

        // special while testing
        public int AppId;

        public Guid Block;
    }

}
