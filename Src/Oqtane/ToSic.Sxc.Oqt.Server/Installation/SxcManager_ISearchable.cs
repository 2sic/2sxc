using Oqtane.Interfaces;
using Oqtane.Models;
using System;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Server.Context;

namespace ToSic.Sxc.Oqt.Server.Installation;

internal partial class SxcManager : ISearchable
{
    public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
    {
        var searchContents = new List<SearchContent>();

        try
        {
            var m = ((OqtModule)moduleGenerator.New()).Init(pageModule.Module);
            searchContents = searchControllerGenerator.New().GetModifiedSearchDocuments(m, lastIndexedOn).ToList();
        }
        catch (Exception e)
        {
            // DnnEnvironmentLogger.AddSearchExceptionToLog(moduleInfo, e, nameof(DnnBusinessController));
        }

        return Task.FromResult(searchContents);
    }
}