using ToSic.Sxc.Render.Sys.ResourceExtractor;

namespace ToSic.Sxc.Tests.Blocks.Output;


public class BlockResourceExtractorGetHtmlAttributesTests
{
 
    private Dictionary<string, string> GetHtmlAttributes(string htmlTag) => BlockResourceExtractor.GetHtmlAttributes(htmlTag).Attributes as Dictionary<string, string>;


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<tag/>")]
    [InlineData("    <tag \n  \n     />    ")]
    //[InlineData("    <   tag      />    ")]
    public void TagWithoutAttributesTest(string htmlTag)
    {
        Null(GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<tag key/>")]
    [InlineData("<tag     key/>")]
    [InlineData("<tag    key    />")]
    [InlineData("<tag \n \t   key    />")]
    public void OneAttributeWithoutValueTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "key", "" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<tag key1 key2 \n key3    Key4  \t  KEY5/>")]
    public void ManyAttributesWithoutValueTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "key1", "" },
            { "key2", "" }, 
            { "key3", "" },
            { "key4", "" },
            { "key5", "" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<tag key1  key1 \n key1    Key1   KEY1/>")]
    public void ManyDuplicateAttributesWithoutValueTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "key1", "" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<tag key=\"value\"/>")]
    [InlineData("<tag key='value'   />")]
    //[InlineData("<tag key=value/>")]
    public void OneAttributeWithValueTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            {"key", "value"}
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<tag key1 key2=\"value\"   \n  key3=\"value\"     />")]
    public void ManyAttributesTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "key1", "" } , 
            { "key2", "value" },
            { "key3", "value" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script type='module' async crossorigin='anonymous' defer integrity='filehash' nomodule='false' referrerpolicy='strict-origin-when-cross-origin'></script>")]
    public void ScriptOnlyAdditionalAttributesTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "type", "module" } ,
            { "async", "" },
            { "crossorigin", "anonymous" },
            { "defer", "" },
            { "integrity", "filehash" },
            { "nomodule", "false" },
            { "referrerpolicy", "strict-origin-when-cross-origin" },
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script async defer></script>")]
    public void ScriptOnlyAdditionalAttributesNoValueTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "async", "" },
            { "defer", "" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script async='async' defer='defer'></script>")]
    public void ScriptOnlyAdditionalAttributesWithValueTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "async", "async" },
            { "defer", "defer" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script id='id' src='src' data-enableoptimizations='true'></script>")]
    public void ScriptOnlyBlacklistAttributesTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>() { };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script id='id' async src='src' defer data-enableoptimizations='true'></script>")]
    public void ScriptMixAttributesTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "async", "" },
            { "defer", "" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script src='src' onload=\"loaded=1\" onerror=\"return false;\"></script>")]
    public void ScriptEventsTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "onload", "loaded=1" },
            { "onerror", "return false;" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script src='src' onerror=\"alert('error!')\"></script>")]
    public void ScriptEventsWithQuotesTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "onerror", "alert('error!')" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script src='src' multiline=\"line1\nli'ne2\nli''ne3\"></script>")]
    public void ScriptMultilineAttributeTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "multiline", "line1\nli'ne2\nli''ne3" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory]
    [InlineData("<script json=\"{&quot;Environment&quot;:{&quot;WebsiteId&quot;:74}}\"></script>")]
    public void ScriptAttributeWithDoubleQuoteSimpleJsonTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "json", "{&quot;Environment&quot;:{&quot;WebsiteId&quot;:74}}" }
        };
        var actual = GetHtmlAttributes(htmlTag);
        /* wip */ Equivalent(expected, actual);
    }

    [Theory]
    [InlineData("<script json='{\"Environment\":{\"WebsiteId\":74}}'></script>")]
    public void ScriptAttributeWithQuoteSimpleJsonTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "json", "{\"Environment\":{\"WebsiteId\":74}}" }
        };
        var actual = GetHtmlAttributes(htmlTag);
        /* wip */ Equivalent(expected, actual);
    }

    [Theory]
    [InlineData("<script json=\"{'Environment':{'WebsiteId':74}}\"></script>")] // 2DM this would never be valid json because json can never use '...' as string holders.
    public void ScriptAttributeWithSingleQuoteSimpleJsonTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "json", "{'Environment':{'WebsiteId':74}}" }
        };
        var actual = GetHtmlAttributes(htmlTag);
        /* wip */ Equivalent(expected, actual);
    }

    [Theory]
    [InlineData("<script json=\"{'Environment':{'WebsiteId':74,'WebsiteUrl':'//2sxc-dnn742.dnndev.me/script-extractor/','PageId':2376,'PageUrl':'http://2sxc-dnn742.dnndev.me/script-extractor','parameters':[],'InstanceId':4139,'SxcVersion':'13.3.0.1646939878','SxcRootUrl':'/','IsEditable':true},'User':{'CanDevelop':true,'CanAdmin':true},'Language':{'Current':'en-us','Primary':'en-us','All':[]},'contentBlockReference':{'publishingMode':'DraftOptional','id':4139,'parentIndex':0,'partOfPage':true},'contentBlock':{'IsCreated':false,'IsList':false,'TemplateId':80568,'QueryId':null,'ContentTypeName':'','AppUrl':'/Portals/script-extractor/2sxc/ScriptExtractorTest','AppSettingsId':null,'AppResourcesId':null,'IsContent':false,'HasContent':false,'SupportsAjax':false,'TemplatePath':'/_v1.cshtml','TemplateIsShared':false,'ZoneId':77,'AppId':852,'Guid':'00000000-0000-0000-0000-000000000000','Id':0},'error':{'type':null},'Ui':{'AutoToolbar':true}}\"></script>")]
    public void ScriptAttributeWithSingleQuoteSxcJsonTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "json", "{'Environment':{'WebsiteId':74,'WebsiteUrl':'//2sxc-dnn742.dnndev.me/script-extractor/','PageId':2376,'PageUrl':'http://2sxc-dnn742.dnndev.me/script-extractor','parameters':[],'InstanceId':4139,'SxcVersion':'13.3.0.1646939878','SxcRootUrl':'/','IsEditable':true},'User':{'CanDevelop':true,'CanAdmin':true},'Language':{'Current':'en-us','Primary':'en-us','All':[]},'contentBlockReference':{'publishingMode':'DraftOptional','id':4139,'parentIndex':0,'partOfPage':true},'contentBlock':{'IsCreated':false,'IsList':false,'TemplateId':80568,'QueryId':null,'ContentTypeName':'','AppUrl':'/Portals/script-extractor/2sxc/ScriptExtractorTest','AppSettingsId':null,'AppResourcesId':null,'IsContent':false,'HasContent':false,'SupportsAjax':false,'TemplatePath':'/_v1.cshtml','TemplateIsShared':false,'ZoneId':77,'AppId':852,'Guid':'00000000-0000-0000-0000-000000000000','Id':0},'error':{'type':null},'Ui':{'AutoToolbar':true}}" }
        };
        /* wip */ Equivalent(expected, GetHtmlAttributes(htmlTag));
    }

    [Theory(Skip = "ATM not ready, Sxc JSON value extraction is not working")]
    [InlineData("<script json=\"{&quot;Environment&quot;:{&quot;WebsiteI&quot;:74,&quot;WebsiteUrl&quot;:&quot;//2sxc-dnn742.dnndev.me/script-extractor/&quot;,&quot;PageId&quot;:2376,&quot;PageUrl&quot;:&quot;http://2sxc-dnn742.dnndev.me/script-extractor&quot;,&quot;parameters&quot;:[],&quot;InstanceId&quot;:4139,&quot;SxcVersion&quot;:&quot;13.3.0.1646939878&quot;,&quot;SxcRootUrl&quot;:&quot;/&quot;,&quot;IsEditable&quot;:true},&quot;User&quot;:{&quot;CanDevelop&quot;:true,&quot;CanAdmin&quot;:true},&quot;Language&quot;:{&quot;Current&quot;:&quot;en-us&quot;,&quot;Primary&quot;:&quot;en-us&quot;,&quot;All&quot;:[]},&quot;contentBlockReference&quot;:{&quot;publishingMode&quot;:&quot;DraftOptional&quot;,&quot;id&quot;:4139,&quot;parentIndex&quot;:0,&quot;partOfPage&quot;:true},&quot;contentBlock&quot;:{&quot;IsCreated&quot;:false,&quot;IsList&quot;:false,&quot;TemplateId&quot;:80568,&quot;QueryId&quot;:null,&quot;ContentTypeName&quot;:&quot;&quot;,&quot;AppUrl&quot;:&quot;/Portals/script-extractor/2sxc/ScriptExtractorTest&quot;,&quot;AppSettingsId&quot;:null,&quot;AppResourcesId&quot;:null,&quot;IsContent&quot;:false,&quot;HasContent&quot;:false,&quot;SupportsAjax&quot;:false,&quot;TemplatePath&quot;:&quot;/_v1.cshtml&quot;,&quot;TemplateIsShared&quot;:false,&quot;ZoneId&quot;:77,&quot;AppId&quot;:852,&quot;Guid&quot;:&quot;00000000-0000-0000-0000-000000000000&quot;,&quot;Id&quot;:0},&quot;error&quot;:{&quot;type&quot;:null},&quot;Ui&quot;:{&quot;AutoToolbar&quot;:true}}\"></script>")]
    //[Ignore("ATM not ready, Sxc JSON value extraction is not working")]
    public void ScriptAttributeWithDoubleQuoteSxcJsonTests(string htmlTag)
    {
        var expected = new Dictionary<string, string>()
        {
            { "json", "{&quot;Environment&quot;:{&quot;WebsiteId&quot;:74,&quot;WebsiteUrl&quot;:&quot;//2sxc-dnn742.dnndev.me/script-extractor/&quot;,&quot;PageId&quot;:2376,&quot;PageUrl&quot;:&quot;http://2sxc-dnn742.dnndev.me/script-extractor&quot;,&quot;parameters&quot;:[],&quot;InstanceId&quot;:4139,&quot;SxcVersion&quot;:&quot;13.3.0.1646939878&quot;,&quot;SxcRootUrl&quot;:&quot;/&quot;,&quot;IsEditable&quot;:true},&quot;User&quot;:{&quot;CanDevelop&quot;:true,&quot;CanAdmin&quot;:true},&quot;Language&quot;:{&quot;Current&quot;:&quot;en-us&quot;,&quot;Primary&quot;:&quot;en-us&quot;,&quot;All&quot;:[]},&quot;contentBlockReference&quot;:{&quot;publishingMode&quot;:&quot;DraftOptional&quot;,&quot;id&quot;:4139,&quot;parentIndex&quot;:0,&quot;partOfPage&quot;:true},&quot;contentBlock&quot;:{&quot;IsCreated&quot;:false,&quot;IsList&quot;:false,&quot;TemplateId&quot;:80568,&quot;QueryId&quot;:null,&quot;ContentTypeName&quot;:&quot;&quot;,&quot;AppUrl&quot;:&quot;/Portals/script-extractor/2sxc/ScriptExtractorTest&quot;,&quot;AppSettingsId&quot;:null,&quot;AppResourcesId&quot;:null,&quot;IsContent&quot;:false,&quot;HasContent&quot;:false,&quot;SupportsAjax&quot;:false,&quot;TemplatePath&quot;:&quot;/_v1.cshtml&quot;,&quot;TemplateIsShared&quot;:false,&quot;ZoneId&quot;:77,&quot;AppId&quot;:852,&quot;Guid&quot;:&quot;00000000-0000-0000-0000-000000000000&quot;,&quot;Id&quot;:0},&quot;error&quot;:{&quot;type&quot;:null},&quot;Ui&quot;:{&quot;AutoToolbar&quot;:true}}" }
        };
        var actual = GetHtmlAttributes(htmlTag);
        /* wip */ Equivalent(expected, actual);
    }
}