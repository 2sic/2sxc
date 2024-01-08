using System.Collections.Generic;
using ToSic.Eav.Apps.Work;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps.Internal.Work;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WorkViewsMod : WorkUnitBase<IAppWorkCtx>
{
    private readonly GenWorkPlus<WorkViews> _appViews;
    private readonly GenWorkDb<WorkEntityCreate> _entityCreate;
    private readonly GenWorkDb<WorkEntityUpdate> _entityUpdate;
    private readonly GenWorkDb<WorkEntityDelete> _entityDelete;

    public WorkViewsMod(
        GenWorkPlus<WorkViews> appViews,
        GenWorkDb<WorkEntityCreate> entityCreate,
        GenWorkDb<WorkEntityUpdate> entityUpdate,
        GenWorkDb<WorkEntityDelete> entityDelete) : base("AWk.EntCre")
    {
        ConnectServices(
            _appViews = appViews,
            _entityCreate = entityCreate,
            _entityUpdate = entityUpdate,
            _entityDelete = entityDelete
        );
    }


    /// <summary>
    /// Adds or updates a template - will create a new template if templateId is not specified
    /// </summary>
    public void CreateOrUpdate(int? templateId, string name, string path, string contentTypeStaticName,
        int? contentDemoEntity, string presentationTypeStaticName, int? presentationDemoEntity,
        string listContentTypeStaticName, int? listContentDemoEntity, string listPresentationTypeStaticName,
        int? listPresentationDemoEntity, string templateType, bool isHidden, string location, bool useForList,
        bool publishData, string streamsToPublish, int? queryEntity, string viewNameInUrl)
    {
        var l = Log.Fn($"{nameof(name)}: {name}");
        var values = new Dictionary<string, object>
        {
            {nameof(IView.Name) /*View.FieldName*/, name },
            {nameof(IView.Path) /*View.FieldPath*/, path },
            {View.FieldContentType, contentTypeStaticName },
            {View.FieldContentDemo, contentDemoEntity.HasValue ? new List<int> { contentDemoEntity.Value } : new List<int>() },
            {View.FieldPresentationType, presentationTypeStaticName },
            {View.FieldPresentationItem, presentationDemoEntity.HasValue ? new List<int> { presentationDemoEntity.Value } : new List<int>() },
            {View.FieldHeaderType, listContentTypeStaticName },
            {View.FieldHeaderItem, listContentDemoEntity.HasValue ? new List<int> { listContentDemoEntity.Value } : new List<int>() },
            {View.FieldHeaderPresentationType, listPresentationTypeStaticName },
            {View.FieldHeaderPresentationItem, listPresentationDemoEntity.HasValue ? new List<int> { listPresentationDemoEntity.Value } : new List<int>() },
            {nameof(IView.Type) /*View.FieldType*/, templateType },
            {nameof(IView.IsHidden) /*View.FieldIsHidden*/, isHidden },
            {View.FieldLocation, location },
            {View.FieldUseList, useForList },
            {View.FieldPublishEnable, publishData },
            {View.FieldPublishStreams, streamsToPublish },
            {View.FieldPipeline, queryEntity.HasValue ? new List<int> { queryEntity.Value } : new List<int>() },
            {View.FieldNameInUrl, viewNameInUrl }
        };


        // #ExtractEntitySave - looks good
        if (templateId.HasValue)
            _entityUpdate.New(AppWorkCtx).UpdateParts(templateId.Value, values);
        else
            _entityCreate.New(AppWorkCtx).Create(Eav.Apps.AppConstants.TemplateContentType, values);

        l.Done();
    }



    public bool DeleteView(int viewId)
    {
        // really get template first, to be sure it is a template
        var template = _appViews.New(AppWorkCtx).Get(viewId);
        return _entityDelete.New(AppWorkCtx).Delete(template.Id);
    }
}