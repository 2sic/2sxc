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

            var contentTypes = Sexy.GetAvailableContentTypes("2SexyContent");
            var templates = Sexy.Templates.GetAllTemplates();
            var entities = DataSource.GetInitialDataSource(ZoneId.Value, AppId.Value, false);
            var language = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            var data = new {
                contentTypes = contentTypes.Select(c => new
                {
                    Id = c.AttributeSetId,
                    Name = c.Name,
                    StaticName = c.StaticName,
                    Templates = templates.Where(t => t.ContentTypeStaticName == c.StaticName).Select(p => new
                    {
                        p.TemplateId,
                        p.ContentTypeStaticName,
                        p.Name
                    }),
                    Entities = entities.List.Where(en => en.Value.Type.AttributeSetId == c.AttributeSetId).Select(en => Sexy.GetDictionaryFromEntity(en.Value, language))
                }),
                templatesWithoutContentType = templates.Where(p => !String.IsNullOrEmpty(p.ContentTypeStaticName)).Select(t => new
                {
                    t.TemplateId,
                    t.Name
                })
            };

            pnlExportView.Attributes.Add("ng-init", "init(" + Newtonsoft.Json.JsonConvert.SerializeObject(data) + ");");

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