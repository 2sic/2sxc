using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Web.Internal.ContentSecurityPolicy;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CspOfPage: ServiceBase
{
    private readonly Generator<CspParameterFinalizer> _cspParameterFinalizer;

    public CspOfPage(Generator<CspParameterFinalizer> cspParameterFinalizer) : base(CspConstants.LogPrefix + ".Page")
    {
        ConnectServices(
            _cspParameterFinalizer = cspParameterFinalizer
        );
    }

    public List<CspParameters> CspParameters { get; } = new();

    public void Add(IList<CspParameters> additional) => CspParameters.AddRange(additional);

    /// <summary>
    /// Name of the CSP header to be added, based on the report-only aspect
    /// </summary>
    public string HeaderName(bool isEnforced) => isEnforced ? CspConstants.CspHeaderNamePolicy : CspConstants.CspHeaderNameReport;


    public string CspHttpHeader()
    {
        try
        {
            var wrapLog = Log.Fn<string>();
            var relevant = CspParameters.Where(cs => cs != null).ToList();
            if (!relevant.Any()) return wrapLog.ReturnNull("none relevant");
            var mergedPolicy = relevant.First();

            var finalizer = _cspParameterFinalizer.New();

            if (relevant.Count == 1)
                return wrapLog.Return(finalizer.Finalize(mergedPolicy).ToString(), "found 1");

            // Pre-copy, so we never change the original!
            mergedPolicy = new CspParameters(mergedPolicy);

            // If many, merge the settings of each additional policy list
            foreach (var cspS in relevant.Skip(1))
                mergedPolicy.Add(cspS);

            return wrapLog.Return(finalizer.Finalize(mergedPolicy).ToString(), "merged");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }


}