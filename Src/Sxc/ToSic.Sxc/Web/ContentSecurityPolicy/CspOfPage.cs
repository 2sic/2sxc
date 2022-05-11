using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    public class CspOfPage: HasLog
    {
        public CspOfPage(): base(CspConstants.LogPrefix + ".Page")
        {
        }

        public List<CspParameters> CspParameters { get; } = new List<CspParameters>();

        public void Add(IList<CspParameters> additional) => CspParameters.AddRange(additional);

        /// <summary>
        /// Name of the CSP header to be added, based on the report-only aspect
        /// </summary>
        public string HeaderName(bool isEnforced) => isEnforced ? CspConstants.CspHeaderNamePolicy : CspConstants.CspHeaderNameReport;


        public string CspHttpHeader()
        {
            try
            {
                var wrapLog = Log.Call<string>();
                var relevant = CspParameters.Where(cs => cs != null).ToList();
                if (!relevant.Any()) return wrapLog("none relevant", null);
                var mergedPolicy = relevant.First();

                var finalizer = new CspParameterFinalizer().Init(Log);

                if (relevant.Count == 1)
                    return wrapLog("found 1", finalizer.Finalize(mergedPolicy).ToString());

                // Pre-copy, so we never change the original!
                mergedPolicy = new CspParameters(mergedPolicy);

                // If many, merge the settings of each additional policy list
                foreach (var cspS in relevant.Skip(1))
                    mergedPolicy.Add(cspS);

                return wrapLog("merged", finalizer.Finalize(mergedPolicy).ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


    }
}
