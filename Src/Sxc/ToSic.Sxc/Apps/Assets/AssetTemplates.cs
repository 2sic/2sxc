using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;


namespace ToSic.Sxc.Apps.Assets
{
    public class Purpose
    {
        // TODO: Once the UIs are updated, we'll kill the 'auto' case
        public const string Auto = "auto";
        //public const string Razor = "razor";
        //public const string Token = "token";
        //public const string Api = "api";
        //public const string Search = "search";
    }

    [PrivateApi]
    public partial class AssetTemplates : ServiceBase
    {
        #region Constants

        internal const string ForTemplate = "Template";
        internal const string ForApi = "Api";
        internal const string ForDataSource = "DataSource";
        private const string ForDocs = "Documentation";
        public const string ForCode = "Code";
        public const string ForSearch = "Search";

        internal const string TypeRazor = "Razor";
        internal const string TypeToken = "Token";
        internal const string TypeNone = "";


        #endregion


        public AssetTemplates() : base("SxcAss.Templt")
        {
        }

        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        public List<TemplateInfo> GetTemplates()
        {
            return _templates ?? (_templates = new List<TemplateInfo>
            {
                RazorHybrid,
                RazorDnn,
                DnnCsCode,
                CsHybrid,
                ApiHybrid,
                DataSourceHybrid,
                Token,
                DnnSearch,
                Markdown,
                EmptyTextFile,
                EmptyFile,
            });
        }
        private static List<TemplateInfo> _templates;

        // TODO: @STV This should probably become obsolete once you change the objects above
        public string GetTemplate(string key) => GetTemplateInfo(key).Body;

        // TODO: @STV This should become obsolete once you change the objects above
        private TemplateInfo GetTemplateInfo(string key) => GetTemplates()
            .FirstOrDefault(t => t.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));


        public const string CsApiTemplateControllerName = "PleaseRenameController";
        public const string CsDataSourceName = "PleaseRename";

    }
}
