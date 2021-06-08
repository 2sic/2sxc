using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;


namespace ToSic.Sxc.Apps.Assets
{
    [PrivateApi]
    public abstract class AssetTemplates: HasLog<IAssetTemplates>, IAssetTemplates
    {
        protected AssetTemplates() : base("SxcAss.Templt")
        {
        }

        public virtual string GetTemplate(AssetTemplateType type)
        {
            var callLog = Log.Call<string>(type.ToString());
            string result;
            switch (type)
            {
                case AssetTemplateType.Unknown:
                    result = "Unknown file type - cannot provide template";
                    break;
                case AssetTemplateType.CsHtml:
                    result = DefaultCshtmlBody;
                    break;
                case AssetTemplateType.CsHtmlCode:
                    result = DefaultCodeCshtmlBody;
                    break;
                case AssetTemplateType.CsCode:
                    result = DefaultCsCode;
                    break;
                case AssetTemplateType.WebApi:
                    result = DefaultWebApiBody;
                    break;
                case AssetTemplateType.Token:
                    result = DefaultTokenHtmlBody;
                    break;
                case AssetTemplateType.CustomSearchCsCode:
                    result = CustomsSearchCsCode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return callLog(null, result);
        }

        internal string DefaultTokenHtmlBody { get; } = @"<p>
    You successfully created your own template.
    Start editing it by hovering the ""Manage"" button and opening the ""Edit Template"" dialog.
</p>";

        internal abstract string DefaultCshtmlBody { get; }

        internal abstract string DefaultCodeCshtmlBody { get; }

        public const string CsApiTemplateControllerName = "PleaseRenameController";

        // copied from the razor tutorial

        internal abstract string DefaultWebApiBody { get; }


        public const string CsCodeTemplateName = "PleaseRenameClass";

        internal abstract string DefaultCsCode { get; }

        internal abstract string CustomsSearchCsCode { get; }


    }
}
