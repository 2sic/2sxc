using System;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Sxc.WebApi.FieldList;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ModuleController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void ChangeOrder(Guid? parent, string fields, int index, int toIndex) 
            => Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log)
                .ChangeOrder(parent, fields, index, toIndex);


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index) 
            => Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log)
                .Publish(part, index);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveFromList(Guid? parent, string fields, int index)
            => Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log)
                .Remove(parent, fields, index);

    }
}
