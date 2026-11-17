using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;

namespace ToSic.Sxc.Dnn;

/// <summary>Logic-free search entry accessors keep test references out of production Find Usages.</summary>
internal static class DnnBusinessControllerTestAccessors
{
    public static IList<SearchDocument> GetModifiedSearchDocumentsTac(this DnnBusinessController controller, ModuleInfo module, DateTime beginDate)
        => controller.GetModifiedSearchDocuments(moduleInfo: module, beginDate: beginDate);
}
