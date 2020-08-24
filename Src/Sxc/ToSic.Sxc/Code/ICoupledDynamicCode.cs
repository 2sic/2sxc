using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    interface ICoupledDynamicCode
    {
        void DynamicCodeCoupling(IDynamicCode parent, string path);

    }
}
