﻿using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        private char OperationShow(bool? show) => show == null ? (char)OprAuto : show.Value ? (char)OprAdd : (char)OprRemove;


        [PrivateApi]
        private string ObjToString(object uiOrParams) => O2U.SerializeIfNotString(uiOrParams);


        private ObjectToUrl O2U => _o2u.Get(() => new ObjectToUrl());
        private readonly GetOnce<ObjectToUrl> _o2u = new GetOnce<ObjectToUrl>();
    }
}