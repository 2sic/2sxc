using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToSic.SexyContent.Interfaces
{
    internal interface IContentBlock
    {
        // bool ContentGroupIsCreated { get;  }
        bool ShowTemplatePicker { get; }

        bool ParentIsEntity { get; }   // alternative is module
        int ParentId { get; }
        string ParentFieldName { get; }
        int ParentFieldSortOrder { get; }

        int AppId { get; }
        Template Template { get; }

        ContentGroup ContentGroup { get; }

        App App { get; }

        IAppData Data { get; }

        bool IsContent { get; }
    }
}
