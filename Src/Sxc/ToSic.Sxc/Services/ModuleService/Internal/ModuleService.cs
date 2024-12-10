using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ModuleService(/*Generator<IModule> moduleGen*/) : ServiceBase(SxcLogName + ".ModSvc"), IModuleService
{
    private readonly Dictionary<int, ModuleServiceData> _moduleData = new();

    public void AddToMore(IHtmlTag tag, string nameId = null, bool noDuplicates = false, int moduleId = default)
    {
        if (tag is null) return;
        nameId ??= tag.ToString();
        var moduleServiceData = GetOrCreateModuleServiceData(moduleId);
        if (noDuplicates && moduleServiceData.ExistingKeys.Contains(nameId)) return;
        moduleServiceData.ExistingKeys.Add(nameId);
        moduleServiceData.MoreTags.Add(tag);
    }

    public IReadOnlyCollection<IHtmlTag> GetAndFlushMoreTags(int moduleId = default)
    {
        //if (moduleId == default) 
        //    moduleId = moduleGen.New().Id;

        var moduleServiceData = GetOrCreateModuleServiceData(moduleId);
        var tags = moduleServiceData.MoreTags;
        _moduleData[moduleId] = new();
        return tags;
    }

    private ModuleServiceData GetOrCreateModuleServiceData(int moduleId = default)
    {
        //if (moduleId == default)
        //    moduleId = moduleGen.New().Id;

        if (_moduleData.TryGetValue(moduleId, out var moduleServiceData)) 
            return moduleServiceData;

        // Handle the case where the moduleId does not exist
        moduleServiceData = new ModuleServiceData();
        _moduleData[moduleId] = moduleServiceData;

        return moduleServiceData;
    }
}