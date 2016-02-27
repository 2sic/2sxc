using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotNetNuke.Common;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.SexyContent.ImportExport;
using System.Web;

namespace ToSic.SexyContent
{
    public partial class Export : SexyControlAdminBase
    {
        private string _scope = "2SexyContent";
        private int _appId;
        private int _zoneId;
        private SexyContent _sexy;

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkCancel.NavigateUrl = Globals.NavigateURL(TabId, "", null);

            _appId = AppId.Value;
            _zoneId = ZoneId.Value;

            if (UserInfo.IsSuperUser)
            {
                _scope = !String.IsNullOrEmpty(Request.QueryString["Scope"]) ? Request.QueryString["Scope"] : "2SexyContent";
                _appId = !String.IsNullOrEmpty(Request.QueryString["AppId"]) ? int.Parse(Request.QueryString["AppId"]) : _appId;
                _zoneId = !String.IsNullOrEmpty(Request.QueryString["ZoneId"]) ? int.Parse(Request.QueryString["ZoneId"]) : _zoneId;
                _sexy = new SexyContent(_zoneId, _appId);
            }
            else
            {
                _sexy = Sexy;
            }

            var contentTypes = _sexy.Templates.GetAvailableContentTypes(_scope, true);
            var templates = _sexy.Templates.GetAllTemplates();
            var entities = DataSource.GetInitialDataSource(_zoneId, _appId, false);
            var language = Thread.CurrentThread.CurrentCulture.Name;

            var data = new {
                contentTypes = contentTypes.Select(c => new
                {
                    Id = c.AttributeSetId, c.Name, c.StaticName,
                    Templates = templates.Where(t => t.ContentTypeStaticName == c.StaticName).Select(p => new
                    {
                        p.TemplateId,
                        p.ContentTypeStaticName,
                        p.Name
                    }),
                    Entities = entities.List.Where(en => en.Value.Type.AttributeSetId == c.AttributeSetId).Select(en => new DynamicEntity(en.Value, new[] { language }, _sexy).ToDictionary() /* _sexy.ToDictionary(en.Value, language) */)
                }),
                templatesWithoutContentType = templates.Where(p => !String.IsNullOrEmpty(p.ContentTypeStaticName)).Select(t => new
                {
                    t.TemplateId,
                    t.Name
                })
            };

            pnlExportView.Attributes.Add("ng-init", "init(" + JsonConvert.SerializeObject(data) + ");");

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            pnlChoose.Visible = false;

            var contentTypeIds = txtSelectedContentTypes.Text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var entityIds = txtSelectedEntities.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            entityIds.AddRange(txtSelectedTemplates.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            //var templateIds = ;

            var messages = new List<ExportImportMessage>();
            var xml = new XmlExporter(_zoneId, _appId, false, contentTypeIds, entityIds.ToArray()).GenerateNiceXml();

            Response.Clear();
            Response.Write(xml);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", "SexyContent-Export.xml"));
            Response.AddHeader("Content-Length", xml.Length.ToString());
            Response.ContentType = "text/xml";
            if (Response.IsClientConnected)
            {
                Response.Flush();
            }
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

    }
}