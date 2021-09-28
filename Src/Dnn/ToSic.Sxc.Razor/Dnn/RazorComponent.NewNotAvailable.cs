using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.Documentation;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn
{
    public abstract partial class RazorComponent
    {
        #region Convert-Service

        public const string NotImplementedUseCustomBase = "Use a newer base class like Custom.Hybrid.Razor12 or Custom.Dnn.Razor12 to leverage this.";

        [PrivateApi]
        public new IConvertService Convert => throw new NotSupportedException($"{nameof(Convert)} not implemented on {nameof(SexyContentWebPage)}. {NotImplementedUseCustomBase}");

        #endregion

    }
}
