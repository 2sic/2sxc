using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApiExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.WebApiExtensions
{
    public abstract class SexyContentApiController : DnnApiController
    {
        private readonly Lazy<SexyContent> _sexyContent;
        private readonly Lazy<SexyContent> _sexyContentUncached;
        private readonly Lazy<IDataSource> _viewData;

        protected SexyContentApiController()
        {
            _sexyContent = new Lazy<SexyContent>(InitSexy);
            _sexyContentUncached = new Lazy<SexyContent>(InitSexyUncached);
            _viewData = new Lazy<IDataSource>(InitViewData);
        }

        private SexyContent InitSexy()
        {
            return HttpRequestMessageExtensions.FindSexy(Request);
        }

        private SexyContent InitSexyUncached()
        {
            return HttpRequestMessageExtensions.FindSexyUncached(Request);
        }

        private IDataSource InitViewData()
        {
            var module = Request.FindModuleInfo();
            return _sexyContent.Value.GetViewDataSource(Request.FindModuleId(), SexyContent.HasEditPermission(module), DotNetNuke.Common.Globals.IsEditMode());
        }

        // Sexy object should not be accessible for other assemblies - just internal use
        internal SexyContent Sexy { get { return _sexyContent.Value; } }
        internal SexyContent SexyUncached { get { return _sexyContentUncached.Value; } }
        public App App { get { return _sexyContent.Value.App; } }
        public IDataSource Data { get { return _viewData.Value; } }
        
    }
}
