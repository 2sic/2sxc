using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.LookUp.Internal;

public abstract class LookUpEngineResolverBase(LazySvc<IHttp> httpLazy, string logName, NoParamOrder protect = default, object[] connect = default)
    : ServiceBase(logName, protect, [..connect ?? [], httpLazy]), ILookUpEngineResolver
{
    public abstract ILookUpEngine GetLookUpEngine(int moduleId);

    /// <summary>
    /// Cache sources by module ID, so we don't have to re-create them every time.
    /// They remain the same throughout a request.
    /// </summary>
    protected readonly Dictionary<int, List<ILookUp>> SourcesByModuleId = [];

    protected LookUpEngine AddToCache(int moduleId, LookUpEngine engine)
    {
        SourcesByModuleId[moduleId] = [.. engine.Sources.Values];
        return engine;
    }

    protected bool TryGetFromCache(int moduleId, out LookUpEngine engine)
    {
        if (!SourcesByModuleId.TryGetValue(moduleId, out var sources))
        {
            engine = null;
            return false;
        }

        engine = new(Log);
        engine.Add(sources);
        return true;
    }


    /// <summary>
    /// Get all http sources which are available in the current context.
    /// But only if they have not already been added to the list.
    /// - QueryString
    /// - Query
    /// - Form (Dnn only)
    /// </summary>
    /// <param name="existingList"></param>
    /// <returns></returns>
    protected List<ILookUp> GetHttpSources(LookUpEngine existingList)
    {
        var l = Log.Fn<List<ILookUp>>($"provider: {existingList.Sources.Count}");
        var http = httpLazy.Value;
        if (http.Current == null)
            return l.Return([], "no http context");

        l.A("Found Http-Context, will ty to add params for querystring, server etc.");

        // Prepare additions to return
        var additions = new List<ILookUp>();
        
        var paramList = http.QueryStringParams;

        // add "query" if it was not already added previously (Oqt has it)
        if (!existingList.HasSource(LookUpConstants.SourceQuery))
            additions.Add(new LookUpInNameValueCollection(LookUpConstants.SourceQuery, paramList));

#if NETFRAMEWORK
        // old (Dnn only)
        if (!existingList.HasSource(LookUpConstants.OldDnnSourceQueryString))
            additions.Add(new LookUpInNameValueCollection(LookUpConstants.OldDnnSourceQueryString, paramList));
        if (!existingList.HasSource("form"))
            additions.Add(new LookUpInNameValueCollection("form", http.Request.Form));
        //provider.Add(new LookUpInNameValueCollection("server", http.Request.ServerVariables)); // deprecated
#else
        // "Not Yet Implemented in .net standard #TodoNetStandard" - might not actually support this
#endif
        return l.Return(additions, $"{additions.Count} additions");
    }
}