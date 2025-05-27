using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Lib.LookUp.Sources;

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