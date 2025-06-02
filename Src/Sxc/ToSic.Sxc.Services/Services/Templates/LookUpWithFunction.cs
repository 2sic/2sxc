using ToSic.Eav.LookUp;

using ToSic.Lib.LookUp.Sources;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Templates;

internal class LookUpWithFunction(string name, Func<string,string> getter): LookUpBase(name, "LookUp using simple function")
{
    public override string Get(string key, string format)
    {
        var result = getter(key);
        return format.IsEmptyOrWs() 
            ? result 
            : string.Format(format, result);
    }
}