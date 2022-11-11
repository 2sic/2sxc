using System;
using ToSic.Eav.Apps.Run;
using ToSic.Lib.Logging;
using ToSic.Sxc.Context;
using ToSic.Sxc.Mvc.Dev;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcModule: IModule
    {
        public MvcModule() {}

        public MvcModule Init(int? tenantId = null, int? id = null, int? appId = null, Guid? block = null)
        {
            TenantId = tenantId ?? TestIds.Blog.Zone;
            Id = id ?? TestIds.Blog.Container;
            AppId = appId ?? TestIds.Blog.App;
            Block = block ?? TestIds.Blog.Block;
            return this;
        }

        // Temp implementation, don't support im MVC
        public IModule Init(int id, ILog parentLog) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public int Id { get; set; }

        public bool IsContent { get; }

        /// <inheritdoc />
        public int TenantId { get; set; }

        /// <inheritdoc />
        public bool IsPrimary => BlockIdentifier.AppId == TestIds.PrimaryApp;

        public IBlockIdentifier BlockIdentifier 
            => _blockIdentifier ??= new BlockIdentifier(TenantId, AppId, Block, Guid.Empty);

        private IBlockIdentifier _blockIdentifier;

        // special while testing
        public int AppId;

        public Guid Block;
    }

}
