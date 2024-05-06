using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Services.LookUp;

internal class LookUpWithFunction(string name, Func<string,string> getter): LookUpBase(name)
{
    public override string Get(string key, string format) 
        => getter(key);
}