using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Integration;

// TODO: @STV I have a feeling this isn't used any where?
internal class OqtModuleHelper(
    IModuleRepository moduleRepository,
    IModuleDefinitionRepository moduleDefinitionRepository)
{
    /// <summary>
    /// Detect is 2sxc Content app
    /// </summary>
    /// <param name="moduleId">module id</param>
    /// <returns>bool</returns>

    public bool IsContentApp(int moduleId)
    {
        var module = moduleRepository.GetModule(moduleId);
        var moduleDefinition = moduleDefinitionRepository.GetModuleDefinitions(module.SiteId).ToList().Find(item => item.ModuleDefinitionName == module.ModuleDefinitionName);
        return moduleDefinition?.Name == "Content";
    }
}