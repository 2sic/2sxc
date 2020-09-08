using System;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.Mvc.WebApi.Context
{
    public class MvcContextBuilder: ContextBuilderBase
    {


        protected override LanguageDto GetLanguage()
        {
            throw new NotImplementedException();
        }

        protected override WebResourceDto GetSystem()
        {
            throw new NotImplementedException();
        }

        protected override WebResourceDto GetSite()
        {
            throw new NotImplementedException();
        }

        protected override WebResourceDto GetPage()
        {
            throw new NotImplementedException();
        }

        protected override EnableDto GetEnable()
        {
            throw new NotImplementedException();
        }

        protected override string GetGettingStartedUrl()
        {
            throw new NotImplementedException();
        }
    }
}
