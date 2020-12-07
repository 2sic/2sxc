using Microsoft.AspNetCore.Mvc;
using System;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.FieldList;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]
    [ValidateAntiForgeryToken]
    //[Authorize(Policy = "EditModule")] // TODO: disabled
    public class ListController : OqtStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.List";

        private readonly Lazy<FieldListBackend> _fieldListBackendLazy;
        private FieldListBackend FieldBacked => _fieldBackend ??= _fieldListBackendLazy.Value.Init(Log);
        private FieldListBackend _fieldBackend;
        public ListController(StatefulControllerDependencies dependencies, Lazy<FieldListBackend> fieldListBackendLazy) : base(dependencies)
        {
            _fieldListBackendLazy = fieldListBackendLazy;
        }

        /// <summary>
        /// used to be GET Module/ChangeOrder
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        /// <param name="toIndex"></param>
        [HttpPost]
        public void Move(Guid? parent, string fields, int index, int toIndex)
            => FieldBacked.ChangeOrder(parent, fields, index, toIndex);

        /// <summary>
        /// Used to be Get Module/RemoveFromList
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        [HttpDelete]
        public void Delete(Guid? parent, string fields, int index)
            => FieldBacked.Remove(parent, fields, index);
    }
}