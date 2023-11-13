using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Web.ContentSecurityPolicy
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CspPolicyTextProcessor: HelperBase
    {
        public CspPolicyTextProcessor(ILog parentLog) : base(parentLog, $"{CspConstants.LogPrefix}.TxtPrc")
        {
        }

        public List<KeyValuePair<string,string>> Parse(string policyText)
        {
            var wrapLog = Log.Fn<List<KeyValuePair<string, string>>>();

            var result = new List<KeyValuePair<string,string>>();
            if (string.IsNullOrEmpty(policyText)) return result;
            var lines = policyText.SplitNewLine()
                // Remove spaces in front and end, + trailing ';'
                .Select(line => line.Trim().TrimEnd(';'))
                // Remove comment lines
                .Where(line => !line.StartsWith("//"))
                // Keep only real lines
                .Where(line => line.HasValue())
                .ToArray();

            foreach (var line in lines)
            {
                var splitIndex = line.IndexOfAny(new[] { ':', ' ' });
                if(splitIndex == -1 || splitIndex >= line.Length) continue;
                var key = line.Substring(0, splitIndex);
                var value = line.Substring(splitIndex + 1).Trim();
                result.Add(new KeyValuePair<string, string>(key, value));
            }

            return wrapLog.Return(result, result.Count.ToString());
        }

    }
}
