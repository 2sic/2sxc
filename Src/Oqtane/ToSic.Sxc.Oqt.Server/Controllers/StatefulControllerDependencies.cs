using Oqtane.Repository;
using System;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// Helper to ensure DI on all controllers, without constantly having to update the parameters on the constructor
    /// </summary>
    public class StatefulControllerDependencies
    {
        public IServiceProvider ServiceProvider { get; }
        public IContextResolver CtxResolver { get; }
        internal readonly IZoneMapper ZoneMapper;
        internal readonly ITenantResolver TenantResolver;
        internal readonly IModuleRepository ModuleRepository;
        //internal readonly IModuleDefinitionRepository ModuleDefinitionRepository;
        //internal readonly ISettingRepository SettingRepository;
        //private readonly OqtaneContainer _oqtaneContainer;
        //internal readonly OqtTempInstanceContext OqtTempInstanceContext;
        internal readonly OqtState OqtState;

        public StatefulControllerDependencies(IZoneMapper zoneMapper,
            ITenantResolver tenantResolver,
            //IModuleDefinitionRepository moduleDefinitionRepository,
            IModuleRepository moduleRepository,
            //ISettingRepository settingRepository,
            //OqtaneContainer oqtaneContainer,
            //OqtTempInstanceContext oqtTempInstanceContext,
            IServiceProvider serviceProvider,
            IContextResolver ctxResolver,
            OqtState oqtState)
        {
            ServiceProvider = serviceProvider;
            CtxResolver = ctxResolver;
            ZoneMapper = zoneMapper;
            TenantResolver = tenantResolver;
            //ModuleDefinitionRepository = moduleDefinitionRepository;
            ModuleRepository = moduleRepository;
            //SettingRepository = settingRepository;
            //_oqtaneContainer = oqtaneContainer;
            //OqtTempInstanceContext = oqtTempInstanceContext;
            OqtState = oqtState;
        }
    }
}
