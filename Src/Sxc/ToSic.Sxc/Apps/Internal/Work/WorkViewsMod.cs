using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal.Work;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WorkViewsMod(
    GenWorkPlus<WorkViews> appViews,
    GenWorkDb<WorkEntityCreate> entityCreate,
    GenWorkDb<WorkEntityUpdate> entityUpdate,
    GenWorkDb<WorkEntityDelete> entityDelete)
    : WorkUnitBase<IAppWorkCtx>("AWk.EntCre", connect: [appViews, entityCreate, entityUpdate, entityDelete])
{
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
            {ViewConstants.FieldContentType, contentTypeStaticName },
            {ViewConstants.FieldContentDemo, contentDemoEntity.HasValue ? [contentDemoEntity.Value] : new List<int>() },
            {ViewConstants.FieldPresentationType, presentationTypeStaticName },
            {ViewConstants.FieldPresentationItem, presentationDemoEntity.HasValue ? [presentationDemoEntity.Value] : new List<int>() },
            {ViewConstants.FieldHeaderType, listContentTypeStaticName },
            {ViewConstants.FieldHeaderItem, listContentDemoEntity.HasValue ? [listContentDemoEntity.Value] : new List<int>() },
            {ViewConstants.FieldHeaderPresentationType, listPresentationTypeStaticName },
            {ViewConstants.FieldHeaderPresentationItem, listPresentationDemoEntity.HasValue ? [listPresentationDemoEntity.Value] : new List<int>() },
            {nameof(IView.Type) /*View.FieldType*/, templateType },
            {nameof(IView.IsHidden) /*View.FieldIsHidden*/, isHidden },
            {ViewConstants.FieldLocation, location },
            {ViewConstants.FieldUseList, useForList },
            {ViewConstants.FieldPublishEnable, publishData },
            {ViewConstants.FieldPublishStreams, streamsToPublish },
            {ViewConstants.FieldPipeline, queryEntity.HasValue ? [queryEntity.Value] : new List<int>() },
            {ViewConstants.FieldNameInUrl, viewNameInUrl }
        };


        // #ExtractEntitySave - looks good
        if (templateId.HasValue)
            entityUpdate.New(AppWorkCtx).UpdateParts(templateId.Value, values);
        else
            entityCreate.New(AppWorkCtx).Create(AppConstants.TemplateContentType, values);

        l.Done();
    }



    public bool DeleteView(int viewId)
    {
        // really get template first, to be sure it is a template
        var template = appViews.New(AppWorkCtx).Get(viewId);
        return entityDelete.New(AppWorkCtx).Delete(template.Id);
    }
}