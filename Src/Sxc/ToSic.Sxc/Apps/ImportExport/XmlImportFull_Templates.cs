using System;
using System.Linq;
using System.Xml.Linq;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.ImportExport;
using ToSic.Lib.Logging;
using ToSic.Eav.Persistence.Logging;
using ToSic.Sxc.Blocks;

// 2dm: must disable NullRef warnings, because there a lot of warnings when processing XML, 
// ...and these are real errors which should blow
// ReSharper disable PossibleNullReferenceException

namespace ToSic.Sxc.Apps.ImportExport
{
    public partial class XmlImportFull
    {

        protected void ImportXmlTemplates(XElement root)
        {
            var l = Log.Fn("import xml templates");
            var templates = root.Element(XmlConstants.Templates);
            if (templates == null) return;

            // The state must come from the DB, and not from the cache
            // Otherwise it will auto-initialize, which it shouldn't do when importing data
            var appState = _repositoryLoader.AppState(AppId, false);

            //var viewsManager = _cmsManagerLazy.Value.InitWithState(appState).Views;
            var viewsMod = _workViewsMod.New(appState);

            foreach (var template in templates.Elements(XmlConstants.Template))
            {
                var name = "";
                try
                {
                    name = template.Attribute(XmlConstants.Name).Value;
                    var path = template.Attribute(nameof(IView.Path)).Value;

                    var contentTypeStaticName = template.Attribute(XmlConstants.AttSetStatic).Value;

                    l.A($"template:{name}, type:{contentTypeStaticName}, path:{path}");

                    if (!string.IsNullOrEmpty(contentTypeStaticName) && appState.GetContentType(contentTypeStaticName) == null)
                    {
                        Messages.Add(new Message($"Content Type for Template \'{name}\' could not be found. The template has not been imported.",
                                Message.MessageTypes.Warning));
                        continue;
                    }

                    var demoEntityGuid = template.Attribute(XmlConstants.TemplateDemoItemGuid).Value;
                    var demoEntityId = new int?();

                    if (!string.IsNullOrEmpty(demoEntityGuid))
                    {
                        var entityGuid = Guid.Parse(demoEntityGuid);
                        if (RepositoryHasEntity(entityGuid))
                            demoEntityId = GetLatestRepositoryId(entityGuid);
                        else
                            Messages.Add(new Message($"Demo Entity for Template \'{name}\' could not be found. (Guid: {demoEntityGuid})", Message.MessageTypes.Information));

                    }

                    var type = template.Attribute(XmlConstants.EntityTypeAttribute).Value;
                    var isHidden = bool.Parse(template.Attribute(nameof(IView.IsHidden)).Value);
                    var location = template.Attribute(View.FieldLocation).Value;
                    var publishData =
                        bool.Parse(template.Attribute(View.FieldPublishEnable) == null
                            ? "False"
                            : template.Attribute(View.FieldPublishEnable).Value);
                    var streamsToPublish = template.Attribute(View.FieldPublishStreams) == null
                        ? ""
                        : template.Attribute(View.FieldPublishStreams).Value;
                    var viewNameInUrl = template.Attribute(View.FieldNameInUrl) == null
                        ? null
                        : template.Attribute(View.FieldNameInUrl).Value;

                    var queryEntityGuid = template.Attribute(XmlConstants.TemplateQueryGuidField);
                    var queryEntityId = new int?();

                    if (!string.IsNullOrEmpty(queryEntityGuid?.Value))
                    {
                        var entityGuid = Guid.Parse(queryEntityGuid.Value);
                        if (RepositoryHasEntity(entityGuid))
                            queryEntityId = GetLatestRepositoryId(entityGuid);
                        else
                            Messages.Add(new Message($"Query Entity for Template \'{name}\' could not be found. (Guid: {queryEntityGuid.Value})", Message.MessageTypes.Information));
                    }

                    var useForList = false;
                    if (template.Attribute(View.FieldUseList) != null)
                        useForList = bool.Parse(template.Attribute(View.FieldUseList).Value);

                    var lstTemplateDefaults = template.Elements(XmlConstants.Entity).Select(e =>
                    {
                        var xmlItemType =
                            e.Elements(XmlConstants.ValueNode)
                                .FirstOrDefault(v =>
                                    v.Attribute(XmlConstants.KeyAttr).Value == XmlConstants.TemplateItemType)?
                                .Attribute(XmlConstants.ValueAttr)
                                .Value;
                        var xmlContentTypeStaticName =
                            e.Elements(XmlConstants.ValueNode)
                                .FirstOrDefault(v =>
                                    v.Attribute(XmlConstants.KeyAttr).Value == XmlConstants.TemplateContentTypeId)?
                                .Attribute(XmlConstants.ValueAttr)
                                .Value;
                        var xmlDemoEntityGuidString =
                            e.Elements(XmlConstants.ValueNode)
                                .FirstOrDefault(v =>
                                    v.Attribute(XmlConstants.KeyAttr).Value == XmlConstants.TemplateDemoItemId)?
                                .Attribute(XmlConstants.ValueAttr)
                                .Value;
                        if (xmlItemType == null || xmlContentTypeStaticName == null || xmlDemoEntityGuidString == null)
                        {
                            Messages.Add(new Message(
                                $"trouble with template '{name}' - either type, static or guid are null",
                                Message.MessageTypes.Error));
                            return null;
                        }
                        var xmlDemoEntityId = new int?();
                        if (xmlDemoEntityGuidString != "0" && xmlDemoEntityGuidString != "")
                        {
                            var xmlDemoEntityGuid = Guid.Parse(xmlDemoEntityGuidString);
                            if (RepositoryHasEntity(xmlDemoEntityGuid))
                                xmlDemoEntityId = GetLatestRepositoryId(xmlDemoEntityGuid);
                        }

                        return new TemplateDefault
                        {
                            ItemType = xmlItemType,
                            ContentTypeStaticName =
                                xmlContentTypeStaticName == "0" || xmlContentTypeStaticName == ""
                                    ? ""
                                    : xmlContentTypeStaticName,
                            DemoEntityId = xmlDemoEntityId
                        };
                    }).ToList();

                    // note: Array lstTemplateDefaults has null objects, so remove null objects
                    var templateDefaults = lstTemplateDefaults.Where(lstItem => lstItem != null).ToList();

                    var presentationTypeStaticName = "";
                    var presentationDemoEntityId = new int?();
                    //if list templateDefaults would have null objects, we would have an exception
                    var presentationDefault = templateDefaults.FirstOrDefault(t => t.ItemType == ViewParts.Presentation);
                    if (presentationDefault != null)
                    {
                        presentationTypeStaticName = presentationDefault.ContentTypeStaticName;
                        presentationDemoEntityId = presentationDefault.DemoEntityId;
                    }

                    var listContentTypeStaticName = "";
                    var listContentDemoEntityId = new int?();
                    var listContentDefault = templateDefaults.FirstOrDefault(t => t.ItemType == ViewParts.FieldHeader);
                    if (listContentDefault != null)
                    {
                        listContentTypeStaticName = listContentDefault.ContentTypeStaticName;
                        listContentDemoEntityId = listContentDefault.DemoEntityId;
                    }

                    var listPresentationTypeStaticName = "";
                    var listPresentationDemoEntityId = new int?();
                    var listPresentationDefault = templateDefaults.FirstOrDefault(t => t.ItemType == ViewParts.FieldHeaderPresentation);
                    if (listPresentationDefault != null)
                    {
                        listPresentationTypeStaticName = listPresentationDefault.ContentTypeStaticName;
                        listPresentationDemoEntityId = listPresentationDefault.DemoEntityId;
                    }

                    viewsMod
                    /*viewsManager*/.CreateOrUpdate(
                        null, name, path, contentTypeStaticName, demoEntityId, presentationTypeStaticName,
                        presentationDemoEntityId, listContentTypeStaticName, listContentDemoEntityId,
                        listPresentationTypeStaticName, listPresentationDemoEntityId, type, isHidden, location,
                        useForList, publishData, streamsToPublish, queryEntityId, viewNameInUrl);

                    Messages.Add(new Message($"Template \'{name}\' successfully imported.",
                        Message.MessageTypes.Information));
                }

                catch (Exception)
                {
                    Messages.Add(new Message($"Import for template \'{name}\' failed.",
                        Message.MessageTypes.Information));
                }

            }

            l.Done();
        }

	}

    internal class TemplateDefault
    {

        public string ItemType;
        public string ContentTypeStaticName;
        public int? DemoEntityId;

    }
}