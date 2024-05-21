using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Services.Templates;

internal class LookUpWithFunctionAndFormat(string name, Func<string,string,string> getter): LookUpBase(name, "LookUp using Function which also handles format")
{
    public override string Get(string key, string format) 
        => getter(key,format);
}