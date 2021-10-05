using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Sxc.Code;


// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12<TModel>: IRazor, IRazor12
    {
        const string DynCode = "_dynCode";

        #region DynCode

        public IDynamicCodeRoot _DynCodeRoot
        {
            get
            {
                // Child razor page will have _dynCode == null, so it is provided via ViewData from parent razor page.
                if (_dynCode == null && ViewData != null && ViewData[DynCode] != null) _dynCode = ViewData[DynCode] as IDynamicCodeRoot;

                return _dynCode;
            }

            set => _dynCode = value;
        }

        // TODO: STV - this should probably also attach logs? not sure in what direction
        public void DynamicCodeCoupling(IDynamicCodeRoot parent)
        {
            _DynCodeRoot = parent;
        }

        private IDynamicCodeRoot _dynCode;
        #endregion

        /// <summary>
        /// Gets or sets the dictionary for view data.
        /// </summary>
        [RazorInject]
        public new ViewDataDictionary<TModel> ViewData
        {
            get => base.ViewData;
            set
            {
                base.ViewData = value;
                // Store _dynCode in ViewData, for child razor page.
                if (_dynCode != null && base.ViewData != null && base.ViewData[DynCode] == null) base.ViewData[DynCode] = _dynCode;
            }
        }

    }
}
