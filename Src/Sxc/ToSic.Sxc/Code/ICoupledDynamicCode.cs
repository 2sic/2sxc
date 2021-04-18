using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    internal interface ICoupledDynamicCode: IHasDynamicCodeRoot
    {
        /// <summary>
        ///  Connect to the parent. Even though it experts an IDynamicCode, it will usually just work properly when it gets an IDynamicCodeRoot
        /// </summary>
        /// <param name="parent"></param>
        void DynamicCodeCoupling(IDynamicCodeRoot parent);

    }
}
