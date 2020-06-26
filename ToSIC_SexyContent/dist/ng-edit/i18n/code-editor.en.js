{
  "Note To Translators": {
    "1": "You probably don't want to translate this",
    "2": "Because it's mostly technical and these users will probably be fine with English.",
    "3": "But if you do translate it, great! Just remember that it means more maintenance"
  },
  "SourceEditorSnippets": {
    "StandardFields": {
      "EntityId.Help": "The id as number of the current entity (content-item)",
      "EntityTitle.Help": "The title of the current entity (content-item) based on the content-type configuration",
      "EntityGuid.Help": "The guid-id of the current entity (content-item)",
      "EntityType.Help": "The type name like 'Person' or 'SimpleContent'",
      "IsPublished.Help": "True/false if this information is published - public user only see published content",
      "Modified.Help": "Internal information when this content-item was last modified"
    },
    "Content": {
      "Title": "Content and Content-Presentation",
      "Help": "The most common data placeholder in a template",
      "General": {
        "Title": "General placeholders",
        "Help": "Various common placeholders",
        "Toolbar.Key": "Toolbar",
        "ToolbarFloat.Key": "Toolbar floating",
        "ToolbarFloat.Help": "Toolbar together with the <div> tag in which it floats",
        "Toolbar.Title": "Hover Toolbars (recommended)",
        "Toolbar.Help": "Hover toolbars let the editor experience the page as it will be",
        "Toolbar.Attrib.Title": "Hover Toolbar Attribute Examples (put inside an html-tag)",
        "Toolbar.Attrib.Help": "These are examples to help you bild cool toolbars. They are always used inside a tag as an attribute - like <div>@Edit.TagToolbar(...)</div>",
        "Toolbar.Inline.Title": "Inline Toolbars (not recommended)",
        "Toolbar.Inline.Help": "Inline toolbars are not recommended, because they change the look of the output for the editor. But sometimes you just need them"
      },
      "Fields.Title": "Fields",
      "Fields.Help": "Fields of the content item as configured in the content-type",
      "PresentationFields.Title": "Presentation fields",
      "PresentationFields.Help": "Presentation settings as configured in the content-type of presentation"
    },
    "List": {
      "Title": "List and List-Presentation",
      "Help": "List functionality in this template - if lists are enabled",
      "Header": {
        "Title": "Header general",
        "Help": "This is the header of a list",
        "Toolbar.Key": "Header toolbar",
        "Toolbar.Help": "Outputs the toolbar to edit list information - place in a <div> to float like other toolbars"
      },
      "Fields.Title": "List fields",
      "Fields.Help": "Fields of the header content-item",
      "PresentationFields.Title": "List presentation fields",
      "PresentationFields.Help": "List presentation settings - usually for settings like paging-size, show-intro, etc.",
      "Repeater": {
        "Title": "Repeaters",
        "Help": "Placeholders as well as loop-templates and more",
        "Repeater.Help": "Allows defining the repeating part of the template"
      },
      "LoopItems": {
        "Title": "Loop Items (inside a repeater)",
        "Help": "Placeholders for things inside a repeater",
        "Index.Help": "Index of the current item",
        "Index1.Help": "Index of the current item + 1 (for numbering lists)",
        "Count.Help": "Count of items in the list",
        "IsFirst.Help": "Outputs 'First' if current item is the first one",
        "IsLast.Help": "Outputs 'Last' if current item is the last one",
        "Alternator2.Help": "Outputs 0 or 1 depending on items index",
        "Alternator3.Help": "Outputs 0, 1 or 2 depending on items index",
        "Alternator4.Help": "Outputs 0, 1, 2 or 3 depending on items index",
        "Alternator5.Help": "Outputs 0, 1, 2, 3 or 4 depending on items index"
      },
      "Sets": [
        {
          "Id": "list",
          "Title": "List Header"
        },
        {
          "Title": "Important List Codes",
          "Items": [
            {
              "<repeat repeat=\"Employee in Data:Default\">...[Employee:...]...</repeat>": "Allows defining the repeating part of the template"
            },
            {
              "[Content:Repeater:Index]": "Index of the current item"
            },
            {
              "[Content:Repeater:Index1]": "Index of the current item + 1 (for numbering lists)"
            },
            {
              "[Content:Repeater:Count]": "Count of items in the list"
            },
            {
              "[Content:Repeater:IsFirst]": "Outputs <b>First<\/b> if current item is the first one"
            },
            {
              "[Content:Repeater:IsLast]": "Outputs <b>Last<\/b> if current item is the last one"
            },
            {
              "[Content:Repeater:Alternator2]": "Outputs <b>0<\/b> or <b>1<\/b> depending on items index"
            },
            {
              "[Content:Repeater:Alternator3]": "Outputs <b>0<\/b>, <b>1<\/b> or <b>2<\/b> depending on items index"
            },
            {
              "[Content:Repeater:Alternator4]": "Outputs <b>0<\/b>, <b>1<\/b>, <b>2<\/b> or <b>3<\/b> depending on items index"
            },
            {
              "[Content:Repeater:Alternator5]": "Outputs <b>0<\/b>, <b>1<\/b>, <b>2<\/b>, <b>3<\/b> or <b>4<\/b> depending on items index"
            },
            {
              "more...": "<a href&#61;\"http:\/\/2sxc.org\/help\">Learn more<\/a>=More help on 2sxc.org\/help"
            }
          ]
        },
        {
          "Id": "listpresentation",
          "Title": "List Presentation"
        }
      ]
    },
    "App": {
      "Title": "App",
      "Help": "App",
      "App.Title": "App",
      "App.Help": "App fields and placeholders",
      "General.Title": "General",
      "General.Help": "General App placeholders",
      "Resources.Title": "App Resources",
      "Resources.Help": "From the app resources content-type which can be configured in the app tab of the app-dialog - usually multi-language",
      "Settings.Title": "App Settings",
      "Settings.Help": "From the app settings content-type which can be configured in the app tab of the app-dialog - usually single-language but can be multi-language"
    },
    "DnnToken": {
      "Title": "Dnn (Portal, Tab, Module, ...)",
      "Help": "Every dnn-token like Portal, Tab, Module, etc.",
      "Portal.Title": "Portal Tokens",
      "Portal.Help": "Portal Tokens",
      "TabTokens.Title": "Tab (Page) Tokens",
      "TabTokens.Help": "Tab (Page) Tokens",
      "ModuleTokens.Title": "Module Tokens",
      "ModuleTokens.Help": "Module Tokens"
    },
    "DnnRazor": {
      "Title": "Dnn objects (module, tab, portal, etc.)",
      "Help": "Find more on https://2sxc.org/en/Docs-Manuals/Feature/feature/3602",
      "Module.Title": "Module",
      "Module.Help": "All module properties",
      "Tab.Title": "Tab (page)",
      "Tab.Help": "All page properties / settings",
      "Portal.Title": "Portal",
      "Portal.Help": "Everything about the portal / zone you are in"
    },
    "User": {
      "Title": "Dnn user information",
      "Help": "Dnn user information",
      "Basic.Title": "Basic user properties",
      "Basic.Help": "Default properties which exist in every DNN",
      "Profile.Title": "Profile",
      "Profile.Help": "Profile information as configured in your portal, like city etc.",
      "Permissions.Title": "Permissions",
      "Permissions.Help": "User permissions"
    },
    "Environment": {
      "Title": "Environment / System",
      "Help": "Environment / System",
      "QueryString.Title": "QueryString (URL-Parameters) Tokens",
      "QueryString.Help": "QueryString (URL-Parameters) Tokens",
      "Time.Title": "Time Tokens",
      "Time.Help": "Time Tokens"
    },
    "C#": {
      "Title": "C# code",
      "Help": "C# code",
      "Basics.Title": "Basics like using or function blocks",
      "Basics.Help": "Basics like using or function blocks",
      "InnerContent.Title": "Inner Content / Very-Rich-Text",
      "InnerContent.Help": "Inner Content / Very-Rich-Text",
      "Headers.Title": "Page headers / titles",
      "Headers.Help": "Page headers / titles"
    },
    "Koi": {
      "Title": "Koi - Multi-CSS Framework Views",
      "Help": "Koi - Multi-CSS Framework Views",
      "Using.Title": "Using statement",
      "Using.Help": "Using statement",
      "CssFramework.Title": "Detect CSS Framework",
      "CssFramework.Help": "Detect CSS Framework",
      "IsUnknown.Title": "Handle Unknown Frameworks",
      "IsUnknown.Help": "Handle Unknown Frameworks",
      "GetFromCdn.Title": "Load missing CSS framework from CDN",
      "GetFromCdn.Help": "Load missing CSS framework from CDN",
      "Class.Title": "Generate Class Attributes",
      "Class.Help": "Generate Class Attributes",
      "If.Title": "If Statements",
      "If.Help": "If Statements",
      "Include.Title": "Include CSS Files",
      "Include.Help": "Include CSS Files"
    },
    "RazorBlade": {
      "Title": "RazorBlade 3",
      "Help": "Read more on https://razor-blade.net",
      "Basics.Title": "Basics",
      "Basics.Help": "Basics like using statements",
      "Text.Title": "Text / String handling",
      "Text.Help": "Various text operations",
      "HtmlPage.Title": "HtmlPage",
      "HtmlPage.Help": "Add icons, headers, etc.",
      "Tags.Title": "Tags - HTML manipulations",
      "Tags.Help": "Modify / manipulate html",
      "Tag.Title": "Tag - generate HTML",
      "Tag.Help": "Generate safe HTML"
    },
    "Linq": {
      "Title": "LINQ - Querying Lists of Data",
      "Help": "LINQ - Querying Lists of Data",
      "Using.Title": "Using statements for LINQ",
      "Using.Help": "Using statements for LINQ",
      "SimpleQueries.Title": "Simple Queries",
      "SimpleQueries.Help": "Simple Queries",
      "Relationships.Title": "Relationship Queries",
      "Relationships.Help": "Relationship Queries"
    },
    "Html": {
      "Title": "Html snippets",
      "Help": "Html snippets",
      "Resources.Title": "Resources like javascripts and css",
      "Resources.Help": "Resources like javascripts and css",
      "2sxcScripts.Title": "2sxc Scripts",
      "2sxcScripts.Help": "2sxc Scripts"
    }
  }
}
