using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Dynamic;
using System.Data;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent
{
    public partial class Export : SexyControlAdminBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            hlkCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId, "", null);

            if (!IsPostBack)
            {
                var ContentTypes = Sexy.ContentContext.GetAllAttributeSets().Where(a => !a.StaticName.StartsWith("@") && a.AttributesInSets.Count > 0 && a.ChangeLogIDDeleted == null && a.Scope == "2SexyContent" && a.App.ZoneID == SexyContent.GetZoneID(PortalId));
                grdContentTypes.DataSource = ContentTypes;
                grdContentTypes.DataBind();

                List<object> Data = new List<object>();
                foreach (var ContentType in ContentTypes)
                {
                    DataTable EntitiesList = Sexy.ContentContext.GetItemsTable(ContentType.AttributeSetID);
                    if (EntitiesList != null)
                    {
                        var Entities = from DataRow i in EntitiesList.Rows
                                       select new
                                       {
                                           ID = i["EntityID"],
                                           Title = i["EntityTitle"],
                                           ContentTypeID = ContentType.AttributeSetID,
                                           ContentTypeName = ContentType.Name
                                       };
                        Data.AddRange(Entities);
                    }
                }
                grdData.DataSource = Data;
                grdData.DataBind();

                grdTemplates.DataSource = from c in Sexy.GetTemplates(this.PortalId)
                                          select new {
                                            c.Name,
                                            c.TemplateID,
                                            c.DemoEntityID
                                          };
                grdTemplates.DataBind();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            pnlChoose.Visible = false;
            pnlExportReport.Visible = true;

            string[] ContentTypeIDs = GetTelerikGridSelections(grdContentTypes);
            string[] EntityIDs = GetTelerikGridSelections(grdData);
            string[] TemplateIDs = GetTelerikGridSelections(grdTemplates);

            List<ExportImportMessage> Messages = new List<ExportImportMessage>();
            string Xml = new XmlExport(ZoneId.Value, AppId.Value, false).ExportXml(ContentTypeIDs, EntityIDs, TemplateIDs, out Messages);
            Response.Clear();
            Response.Write(Xml);
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", "SexyContent-Export.xml"));
            Response.AddHeader("Content-Length", Xml.Length.ToString());
            Response.ContentType = "text/xml";
            Response.End();
        }

        private string[] GetTelerikGridSelections(Telerik.Web.UI.RadGrid Grid)
        {
            string Key = Grid.MasterTableView.DataKeyNames[0];
            string[] SelectedIDs = (from Telerik.Web.UI.GridItem c in Grid.SelectedItems
                                      select Grid.MasterTableView.DataKeyValues[c.ItemIndex][Key].ToString()).ToArray();
            return SelectedIDs;
        }
    }
}