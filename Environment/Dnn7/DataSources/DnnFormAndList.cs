using System.Collections.Generic;
using DotNetNuke.Modules.UserDefinedTable;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.Eav.Types.Attributes;

namespace ToSic.SexyContent.Environment.Dnn7.DataSources
{
    /// <inheritdoc />
    /// <summary>
    /// Delivers UDT-data (now known as Form and List) to the templating engine
    /// </summary>
    [PipelineDesigner]
    [ExpectsDataOfType("|Config ToSic.SexyContent.DataSources.DnnFormAndList")]
    public class DnnFormAndList : BaseDataSource
    {
        private DataTableDataSource DtDs;

        #region Configuration-properties
        public override string LogId => "DS-FnL";

        private const string ModuleIdKey = "ModuleId";
        private const string TitleFieldKey = "TitleField";
        private const string ContentTypeKey = "ContentType";
        private const string EntityTitleDefaultKeyToken = "[Settings:TitleFieldName]";
        private const string FnLModuleIdDefaultToken = "[Settings:ModuleId||0]";
        private const string ContentTypeDefaultToken = "[Settings:ContentTypeName||FnL]";

        /// <summary>
        /// Gets or sets the FnL ModuleID containing the data
        /// </summary>
        public int ModuleId
        {
            get => int.Parse(Configuration[ModuleIdKey]);
            set => Configuration[ModuleIdKey] = value.ToString();
        }

        /// <summary>
        /// Gets or sets the Name of the Title Attribute of the Source DataTable
        /// </summary>
        public string TitleField
        {
            get => Configuration[TitleFieldKey];
            set => Configuration[TitleFieldKey] = value;
        }

        /// <summary>
        /// Gets or sets the Name of the ContentType Attribute 
        /// </summary>
        public string ContentType 
        {
            get => Configuration[ContentTypeKey];
            set => Configuration[ContentTypeKey] = value;
        }
        #endregion

        // Todo:
        // - Could also supply modified/created dates if needed from the FnL...

        /// <summary>
        /// Initializes a new instance of FormAndListDataSource class
        /// </summary>
        public DnnFormAndList()
        {
            Out.Add(Constants.DefaultStreamName, new DataStream(this, Constants.DefaultStreamName, GetEntities, GetList));
            Configuration.Add(ModuleIdKey, FnLModuleIdDefaultToken);
            Configuration.Add(TitleFieldKey, EntityTitleDefaultKeyToken);
            Configuration.Add(ContentTypeKey, ContentTypeDefaultToken);
        }


        private void LoadFnL()
        {
             EnsureConfigurationIsLoaded();

            // Preferred way in Form and List
            var udt = new UserDefinedTableController();
            var ds = udt.GetDataSet(ModuleId);
            DtDs = DataSource.GetDataSource<DataTableDataSource>(valueCollectionProvider: ConfigurationProvider, parentLog:Log);
            DtDs.Source = ds.Tables["Data"];
            DtDs.EntityIdField = "UserDefinedRowId";         // default column created by UDT
            DtDs.ContentType = ContentType;

            // clean up column names if possible, remove spaces in the column-names
            for (var i = 0; i < DtDs.Source.Columns.Count; i++)
                DtDs.Source.Columns[i].ColumnName = DtDs.Source.Columns[i].ColumnName
                    .Replace(" ", "");

            // Set the title-field - either the configured one, or if missing, just the first column we find
            if (string.IsNullOrWhiteSpace(TitleField))
                TitleField = DtDs.Source.Columns[1].ColumnName;  
            DtDs.TitleField = TitleField;
        }

        /// <summary>
        /// Internal helper that returns the entities - actually just retrieving them from the attached Data-Source
        /// </summary>
        /// <returns></returns>
        private IDictionary<int, ToSic.Eav.Interfaces.IEntity> GetEntities()
        {
            // if not initialized, do that first
            if (DtDs == null)
                LoadFnL();

            return DtDs["Default"].List;
        }

        /// <summary>
        /// Internal helper that returns the entities - actually just retrieving them from the attached Data-Source
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ToSic.Eav.Interfaces.IEntity> GetList()
        {
            // if not initialized, do that first
            if (DtDs == null)
                LoadFnL();

            return DtDs["Default"].LightList;
        }
    }
}
