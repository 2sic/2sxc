using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Very internal functionality, could change at any time")]
    internal interface ICanGetByName
    {
        dynamic Get(string name);
    }
}
