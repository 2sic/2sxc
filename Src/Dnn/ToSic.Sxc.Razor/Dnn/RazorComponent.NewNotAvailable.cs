using System;
using ToSic.Eav.Documentation;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn
{
    public abstract partial class RazorComponent
    {

        public const string NotImplementedUseCustomBase = "Use a newer base class like Custom.Hybrid.Razor12 or Custom.Dnn.Razor12 to leverage this.";

        //#region Convert-Service
        //[PrivateApi]
        //public new IxConvertService Convert => throw new NotSupportedException($"{nameof(Convert)} not implemented on {nameof(SexyContentWebPage)}. {NotImplementedUseCustomBase}");
        //#endregion

    }
}
