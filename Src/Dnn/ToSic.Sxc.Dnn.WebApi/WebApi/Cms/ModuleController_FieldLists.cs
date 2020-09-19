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
        private FieldListBackend FieldBacked => Factory.Resolve<FieldListBackend>().Init(GetContext(), GetBlock(), Log);

        // Moved to ListController
        //[HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        //public void ChangeOrder(Guid? parent, string fields, int index, int toIndex)
        //    => FieldBacked.ChangeOrder(parent, fields, index, toIndex);


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index) 
            => FieldBacked.Publish(part, index);

        // Moved to ListController
        //[HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        //public void RemoveFromList(Guid? parent, string fields, int index)
        //    => FieldBacked.Remove(parent, fields, index);

    }
}
