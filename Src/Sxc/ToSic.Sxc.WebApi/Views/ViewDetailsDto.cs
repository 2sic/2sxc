using System;
using System.Collections.Generic;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Security;

namespace ToSic.Sxc.WebApi.Views
{
    public class ViewDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ViewContentTypeDto ContentType { get; set; }
        public ViewContentTypeDto PresentationType { get; set; }
        public ViewContentTypeDto ListContentType { get; set; }
        public ViewContentTypeDto ListPresentationType { get; set; }
        public string TemplatePath { get; set; }
        public bool IsHidden { get; set; }
        public string ViewNameInUrl { get; set; }
        public Guid Guid { get; set; }
        public bool List { get; set; }
        public bool HasQuery { get; set; }
        public int Used { get; set; }

        public bool IsShared { get; set; }

        public IEnumerable<EavLightEntityReference> Metadata { get; set; }

        public HasPermissionsDto Permissions { get; set; }
    }
}
