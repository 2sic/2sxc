using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Dynamic;
using System.Data;
using DotNetNuke.Common.Utilities;
using ToSic.Eav;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent
{
    public partial class Export : SexyControlAdminBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId, "", null);

            var contentTypes = Sexy.GetAvailableAttributeSets("2SexyContent");
            var templates = Sexy.GetTemplates(PortalSettings.PortalId);
            var entities = DataSource.GetInitialDataSource(ZoneId.Value, AppId.Value, false);
            var language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            var data = new {
                contentTypes = contentTypes.Select(c => new
                {
                    Id = c.AttributeSetID,
                    Name = c.Name,
                    StaticName = c.StaticName,
                    Templates = templates.Where(t => t.AttributeSetID == c.AttributeSetID).Select(p => new
                    {
                        p.TemplateID,
                        p.AttributeSetID,
                        p.Name,
                        TemplateDefaults = Sexy.GetTemplateDefaults(p.TemplateID).Select(td => new
                        {
                            td.ContentTypeID,
                            td.DemoEntityID,
                            ItemType = td.ItemType.ToString("F")
                        })
                    }),
                    Entities = entities.List.Where(en => en.Value.Type.AttributeSetId == c.AttributeSetID).Select(en => Sexy.GetDictionaryFromEntity(en.Value, language))
                }),
                templatesWithoutContentType = templates.Where(p => !p.AttributeSetID.HasValue).Select(t => new
                {
                    t.TemplateID,
                    t.Name
                })
            };

            pnlExportView.Attributes.Add("ng-init", "init(" + data.ToJson() + ");");

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            pnlChoose.Visible = false;

            string[] contentTypeIds = txtSelectedContentTypes.Text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            string[] entityIds = txtSelectedEntities.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] templateIds = txtSelectedTemplates.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var messages = new List<ExportImportMessage>();
            var xml = new XmlExport(ZoneId.Value, AppId.Value, false).ExportXml(contentTypeIds, entityIds, templateIds, out messages);

            Response.Clear();
            Response.Write(xml);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", "SexyContent-Export.xml"));
            Response.AddHeader("Content-Length", xml.Length.ToString());
            Response.ContentType = "text/xml";
            Response.End();
        }

    }
}