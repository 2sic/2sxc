using System.Linq;
using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Integration;

// TODO: @STV I have a feeling this isn't used any where?
internal class OqtModuleHelper
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IModuleDefinitionRepository _moduleDefinitionRepository;

    public OqtModuleHelper(IModuleRepository moduleRepository, IModuleDefinitionRepository moduleDefinitionRepository)
    {
        _moduleRepository = moduleRepository;
        _moduleDefinitionRepository = moduleDefinitionRepository;
    }

    /// <summary>
    /// Detect is 2sxc Content app
    /// </summary>
    /// <param name="moduleId">module id</param>
    /// <returns>bool</returns>

    public bool IsContentApp(int moduleId)
    {
        var module = _moduleRepository.GetModule(moduleId);
        var moduleDefinition = _moduleDefinitionRepository.GetModuleDefinitions(module.SiteId).ToList().Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);
        return moduleDefinition?.Name == "Content";
    }
}