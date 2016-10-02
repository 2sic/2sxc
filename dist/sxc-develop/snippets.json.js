{
    "snippets": [
        {
            "set": "@App",
            "subset": "App",
            "name": "Path",
            "title": "returns the url to the current app",
            "content": "@App.Path",
            "help": "path for integrating scripts,  images etc. For example  use as @App.Path/scripts/knockout.js",
            "links": "read API docs: https://github.com/2sic/2sxc/wiki/Razor-App"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "PhysPath",
            "title": "physical path",
            "content": "@App.PhysicalPath",
            "help": "physical path in c:\\",
            "links": "read API docs: https://github.com/2sic/2sxc/wiki/Razor-App"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppGuid",
            "title": "App Guid",
            "content": "@App.AppGuid",
            "help": "internal GUID - should stay the same across all systems for this specific App",
            "links": "read API docs: https://github.com/2sic/2sxc/wiki/Razor-App"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppId",
            "title": "App Id",
            "content": "@App.AppId",
            "help": "Id in the current data base. Is a different number in every App-Installation",
            "links": "read API docs: https://github.com/2sic/2sxc/wiki/Razor-App"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppName",
            "title": "App Name",
            "content": "@App.Name",
            "help": "internal name",
            "links": "read API docs: https://github.com/2sic/2sxc/wiki/Razor-App"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppFolder",
            "title": "folder of the 2sxc-app",
            "content": "@App.Folder",
            "help": "often used to create paths to scripts or join some values. if you only need to reference a script,  please use App.Path",
            "links": "read API docs: https://github.com/2sic/2sxc/wiki/Razor-App"
        },
        {
            "set": "@Content",
            "subset": "General",
            "name": "Toolbar for an item",
            "title": "",
            "content": "@Edit.Toolbar(${1:Content})",
            "help": "Show an inline-toolbar. If you want it hovering, make sure you have an HTML-element around it with the class sc-element.",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Edit"
        },
        {
            "set": "@Content",
            "subset": "General",
            "name": "ToolbarFloat for an item",
            "title": "",
            "content": "<div class=\"sc-element\">\r\n    @Edit.Toolbar(${1:Content})\r\n</div>",
            "help": "Show an inline toolbar, floating. Note that this is just an example, the div with the class sc-element can be further away, it doesn't have to be the direct container. ",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Edit"
        },
        {
            "set": "@Content",
            "subset": "General",
            "name": "Toolbar for item with edit / replace only",
            "title": "",
            "content": "@Edit.Toolbar(${1:Content}, actions: \"edit,replace\")",
            "help": "Toolbar with edit and replace buttons only",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Edit"
        },
        {
            "set": "@Content",
            "subset": "General",
            "name": "Toolbar to create new only",
            "title": "",
            "content": "@Edit.Toolbar(actions: \"new\", contentType: \"${10:BlogPost}\")",
            "help": "Toolbar with edit and replace buttons only",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Edit"
        },
        {
            "set": "@Content",
            "subset": "General",
            "name": "Toolbar to create new and prefill",
            "title": "",
            "content": "@Edit.Toolbar(actions: \"new\", contentType: \"${10:BlogPost}\", prefill: new { Title = \"Hello\", Color = \"red\" } )",
            "help": "Toolbar with new and prefill example",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Edit"
        },
        {
            "set": "@List",
            "subset": "Header",
            "name": "List-Toolbar",
            "title": "Header toolbar",
            "content": "@Edit.Toolbar(ListContent)",
            "help": "Outputs the toolbar to edit list information - place in a <div> to float like other toolbars",
            "links": ""
        },
        {
            "set": "@List",
            "subset": "Header",
            "name": "List-ToolbarFloat",
            "title": "",
            "content": "<div class=\"sc-element\">\r\n    @Edit.Toolbar(ListContent)\r\n</div>",
            "help": "",
            "links": ""
        },
        {
            "set": "@List",
            "subset": "Repeater",
            "name": "foreach loop",
            "title": "for-each on the default list",
            "content": "@foreach(var ${1:cont} in AsDynamic(Data[\"${2:Default}\"])){\r\n    <div class=\"sc-element\">\r\n        @${1}.EntityTitle\r\n        @$Edit.Toolbar({1})\r\n    </div>\r\n}",
            "help": "simple loop to show all items in the default list",
            "links": "read API docs: https://github.com/2sic/2sxc/wiki/Razor-Data"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "AllModules",
            "title": "",
            "content": "@Dnn.Module.AllModules",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "AllTabs",
            "title": "",
            "content": "@Dnn.Module.AllTabs",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Border",
            "title": "",
            "content": "@Dnn.Module.Border",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Cacheability",
            "title": "",
            "content": "@Dnn.Module.Cacheability",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "CacheMethod",
            "title": "",
            "content": "@Dnn.Module.CacheMethod",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "CacheTime",
            "title": "",
            "content": "@Dnn.Module.CacheTime",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ContainerPath",
            "title": "",
            "content": "@Dnn.Module.ContainerPath",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ContainerSrc",
            "title": "",
            "content": "@Dnn.Module.ContainerSrc",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DefaultCacheTime",
            "title": "",
            "content": "@Dnn.Module.DefaultCacheTime",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DesktopModule",
            "title": "",
            "content": "@Dnn.Module.DesktopModule",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DesktopModuleId",
            "title": "",
            "content": "@Dnn.Module.DesktopModuleId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DisplayTitle",
            "title": "",
            "content": "@Dnn.Module.DisplayTitle",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "EndDate",
            "title": "",
            "content": "@Dnn.Module.EndDate",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Footer",
            "title": "",
            "content": "@Dnn.Module.Footer",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Header",
            "title": "",
            "content": "@Dnn.Module.Header",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleControl",
            "title": "",
            "content": "@Dnn.Module.ModuleControl",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleControlId",
            "title": "",
            "content": "@Dnn.Module.ModuleControlId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleDefID",
            "title": "",
            "content": "@Dnn.Module.ModuleDefID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleDefinition",
            "title": "",
            "content": "@Dnn.Module.ModuleDefinition",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleID",
            "title": "",
            "content": "@Dnn.Module.ModuleID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleName",
            "title": "",
            "content": "@Dnn.Module.ModuleName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleOrder",
            "title": "",
            "content": "@Dnn.Module.ModuleOrder",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModulePermissions",
            "title": "",
            "content": "@Dnn.Module.ModulePermissions",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleSettings",
            "title": "",
            "content": "@Dnn.Module.ModuleSettings",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleTitle",
            "title": "",
            "content": "@Dnn.Module.ModuleTitle",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PaneModuleCount",
            "title": "",
            "content": "@Dnn.Module.PaneModuleCount",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PaneModuleIndex",
            "title": "",
            "content": "@Dnn.Module.PaneModuleIndex",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PaneName",
            "title": "",
            "content": "@Dnn.Module.PaneName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ParentTab",
            "title": "",
            "content": "@Dnn.Module.ParentTab",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PortalID",
            "title": "",
            "content": "@Dnn.Module.PortalID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "StartDate",
            "title": "",
            "content": "@Dnn.Module.StartDate",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "TabID",
            "title": "",
            "content": "@Dnn.Module.TabID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "TabModuleID",
            "title": "",
            "content": "@Dnn.Module.TabModuleID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "TabModuleSettings",
            "title": "",
            "content": "@Dnn.Module.TabModuleSettings",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "UniqueId",
            "title": "",
            "content": "@Dnn.Module.UniqueId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Visibility",
            "title": "",
            "content": "@Dnn.Module.Visibility",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ChildModules",
            "title": "",
            "content": "@Dnn.Tab.ChildModules",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ContainerPath",
            "title": "",
            "content": "@Dnn.Tab.ContainerPath",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ContainerSrc",
            "title": "",
            "content": "@Dnn.Tab.ContainerSrc",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "CultureCode",
            "title": "",
            "content": "@Dnn.Tab.CultureCode",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "DefaultLanguageGuid",
            "title": "",
            "content": "@Dnn.Tab.DefaultLanguageGuid",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "DefaultLanguageTab",
            "title": "",
            "content": "@Dnn.Tab.DefaultLanguageTab",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Description",
            "title": "",
            "content": "@Dnn.Tab.Description",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "EndDate",
            "title": "",
            "content": "@Dnn.Tab.EndDate",
            "help": "for automatic hiding of the page",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "FullUrl",
            "title": "",
            "content": "@Dnn.Tab.FullUrl",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "HasChildren",
            "title": "",
            "content": "@Dnn.Tab.HasChildren",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsDefaultLanguage",
            "title": "",
            "content": "@Dnn.Tab.IsDefaultLanguage",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsDeleted",
            "title": "",
            "content": "@Dnn.Tab.IsDeleted",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsNeutralCulture",
            "title": "",
            "content": "@Dnn.Tab.IsNeutralCulture",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsSecure",
            "title": "",
            "content": "@Dnn.Tab.IsSecure",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsTranslated",
            "title": "",
            "content": "@Dnn.Tab.IsTranslated",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsVisible",
            "title": "",
            "content": "@Dnn.Tab.IsVisible",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "KeyWords",
            "title": "",
            "content": "@Dnn.Tab.KeyWords",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Level",
            "title": "",
            "content": "@Dnn.Tab.Level",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "LocalizedTabName",
            "title": "",
            "content": "@Dnn.Tab.LocalizedTabName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "LocalizedTabs",
            "title": "",
            "content": "@Dnn.Tab.LocalizedTabs",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Modules",
            "title": "",
            "content": "@Dnn.Tab.Modules",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "PageHeadtext",
            "title": "",
            "content": "@Dnn.Tab.PageHeadtext",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Panes",
            "title": "",
            "content": "@Dnn.Tab.Panes",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ParentId",
            "title": "",
            "content": "@Dnn.Tab.ParentId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "PermanentRedirect",
            "title": "",
            "content": "@Dnn.Tab.PermanentRedirect",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "PortalID",
            "title": "",
            "content": "@Dnn.Tab.PortalID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "RefreshInterval",
            "title": "",
            "content": "@Dnn.Tab.RefreshInterval",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SiteMapPriority",
            "title": "",
            "content": "@Dnn.Tab.SiteMapPriority",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SkinDoctype",
            "title": "",
            "content": "@Dnn.Tab.SkinDoctype",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SkinPath",
            "title": "",
            "content": "@Dnn.Tab.SkinPath",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SkinSrc",
            "title": "",
            "content": "@Dnn.Tab.SkinSrc",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "StartDate",
            "title": "",
            "content": "@Dnn.Tab.StartDate",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabID",
            "title": "",
            "content": "@Dnn.Tab.TabID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabName",
            "title": "",
            "content": "@Dnn.Tab.TabName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabOrder",
            "title": "",
            "content": "@Dnn.Tab.TabOrder",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabPath",
            "title": "",
            "content": "@Dnn.Tab.TabPath",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabPermissions",
            "title": "",
            "content": "@Dnn.Tab.TabPermissions",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabSettings",
            "title": "",
            "content": "@Dnn.Tab.TabSettings",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabType",
            "title": "",
            "content": "@Dnn.Tab.TabType",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Title",
            "title": "",
            "content": "@Dnn.Tab.Title",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "UniqueId",
            "title": "",
            "content": "@Dnn.Tab.UniqueId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Url",
            "title": "",
            "content": "@Dnn.Tab.Url",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "ActiveTab",
            "title": "",
            "content": "@Dnn.Portal.ActiveTab",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdministratorId",
            "title": "",
            "content": "@Dnn.Portal.AdministratorId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdministratorRoleId",
            "title": "",
            "content": "@Dnn.Portal.AdministratorRoleId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdministratoRoleName",
            "title": "",
            "content": "@Dnn.Portal.AdministratoRoleName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdminTabId",
            "title": "",
            "content": "@Dnn.Portal.AdminTabId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "CultureCode",
            "title": "",
            "content": "@Dnn.Portal.CultureCode",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Currency",
            "title": "",
            "content": "@Dnn.Portal.Currency",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Current",
            "title": "",
            "content": "@Dnn.Portal.Current",
            "help": "static method returning the current portal portal-settings",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Default",
            "title": "",
            "content": "@Dnn.Portal.Default",
            "help": "(AdminContainer, AdminSkin, ControlPanelMode, etc.)",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultLanguage",
            "title": "",
            "content": "@Dnn.Portal.DefaultLanguage",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultModuleId",
            "title": "",
            "content": "@Dnn.Portal.DefaultModuleId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultPortalContainer",
            "title": "",
            "content": "@Dnn.Portal.DefaultPortalContainer",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultTabId",
            "title": "",
            "content": "@Dnn.Portal.DefaultTabId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Description",
            "title": "",
            "content": "@Dnn.Portal.Description",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Email",
            "title": "",
            "content": "@Dnn.Portal.Email",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "EnableBrowserLanguage",
            "title": "",
            "content": "@Dnn.Portal.EnableBrowserLanguage",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "EnableUrlLanguage",
            "title": "",
            "content": "@Dnn.Portal.EnableUrlLanguage",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "ExpiryDate",
            "title": "",
            "content": "@Dnn.Portal.ExpiryDate",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "FooterText",
            "title": "",
            "content": "@Dnn.Portal.FooterText",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Guid",
            "title": "",
            "content": "@Dnn.Portal.GUID - notice all large letters",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "HomeDirectory",
            "title": "",
            "content": "@Dnn.Portal.HomeDirectory",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "HomeDirectoryMapPath",
            "title": "",
            "content": "@Dnn.Portal.HomeDirectoryMapPath",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "HomeTabId",
            "title": "",
            "content": "@Dnn.Portal.HomeTabId",
            "help": "the root page",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "KeyWords",
            "title": "",
            "content": "@Dnn.Portal.KeyWords",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "LoginTabId",
            "title": "",
            "content": "@Dnn.Portal.LoginTabId",
            "help": "where the normal login usually goes to",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "LogoFile",
            "title": "",
            "content": "@Dnn.Portal.LogoFile",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Pages",
            "title": "",
            "content": "@Dnn.Portal.Pages",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalAlias",
            "title": "",
            "content": "@Dnn.Portal.PortalAlias",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalId",
            "title": "",
            "content": "@Dnn.Portal.PortalId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalName",
            "title": "",
            "content": "@Dnn.Portal.PortalName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalSkin",
            "title": "",
            "content": "@Dnn.Portal.PortalSkin",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "RegisteredRoleId",
            "title": "",
            "content": "@Dnn.Portal.RegisteredRoleId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "RegisteredRoleName",
            "title": "",
            "content": "@Dnn.Portal.RegisteredRoleName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "RegisterTabId",
            "title": "",
            "content": "@Dnn.Portal.RegisterTabId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Search",
            "title": "",
            "content": "@Dnn.Portal.Search.something",
            "help": "various values like IncludeCommon, IncludeTagInfoFilter etc.",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SearchTabId",
            "title": "",
            "content": "@Dnn.Portal.SearchTabId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SiteLogHistory",
            "title": "",
            "content": "@Dnn.Portal.SiteLogHistory",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SplashTabId",
            "title": "",
            "content": "@Dnn.Portal.SplashTabId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SSLEnabled",
            "title": "",
            "content": "@Dnn.Portal.SSLEnabled",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SSLEnforced",
            "title": "",
            "content": "@Dnn.Portal.SSLEnforced",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SSLURL",
            "title": "",
            "content": "@Dnn.Portal.SSLURL",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "STDURL",
            "title": "",
            "content": "@Dnn.Portal.STDURL",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SuperTabId",
            "title": "",
            "content": "@Dnn.Portal.SuperTabId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "TimeZoneOffset",
            "title": "",
            "content": "@Dnn.Portal.TimeZoneOffset",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserId",
            "title": "",
            "content": "@Dnn.Portal.UserId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserInfo",
            "title": "",
            "content": "@Dnn.Portal.UserInfo",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserMode",
            "title": "",
            "content": "@Dnn.Portal.UserMode",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserQuota",
            "title": "",
            "content": "@Dnn.Portal.UserQuota",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserRegistration",
            "title": "",
            "content": "@Dnn.Portal.UserRegistration",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Users",
            "title": "",
            "content": "@Dnn.Portal.Users",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserTabId",
            "title": "",
            "content": "@Dnn.Portal.UserTabId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "DisplayName",
            "title": "",
            "content": "@Dnn.User.DisplayName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Email",
            "title": "",
            "content": "@Dnn.User.Email",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "FirstName",
            "title": "",
            "content": "@Dnn.User.FirstName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "FullName",
            "title": "",
            "content": "@Dnn.User.FullName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "IsDeleted",
            "title": "",
            "content": "@Dnn.User.IsDeleted",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "IsInRole",
            "title": "",
            "content": "@Dnn.User.IsInRole(string RoleName)",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "IsSuperUser",
            "title": "",
            "content": "@Dnn.User.IsSuperUser",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "LastName",
            "title": "",
            "content": "@Dnn.User.LastName",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Membership",
            "title": "",
            "content": "@Dnn.User.Membership",
            "help": "these are asp.net memberships, you probably don't need them",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "PortalId",
            "title": "",
            "content": "@Dnn.User.PortalId",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Roles",
            "title": "",
            "content": "@Dnn.User.Roles",
            "help": "these are DNN roles",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "UserID",
            "title": "",
            "content": "@Dnn.User.UserID",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Username",
            "title": "",
            "content": "@Dnn.User.Username",
            "help": "",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Profile",
            "name": "AnyValue",
            "title": "",
            "content": "@Dnn.User.Profile.GetPropertyValue(\\${1:City}\\)",
            "help": "any property of the user profile as configured in your portal",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Permissions",
            "name": "IsEditMode",
            "title": "",
            "content": "@if (DotNetNuke.Common.Globals.IsEditMode())\r\n{\r\n        <div> stuff here which only appears in edit mode </div>\r\n}",
            "help": "",
            "links": ""
        },
        {
            "set": "@User",
            "subset": "Permissions",
            "name": "UserMayEditContent",
            "title": "",
            "content": "@if(Permissions.UserMayEditContent)\r\n{\r\n       <!-- stuff, like a custom edit toolbar - here an example -->\r\n       <ul class=\"sc-menu\" data-toolbar='{ \"action\": \"new\", \"contentType\": \"Gallery Image\", \"prefill\": { \"File\": \"File:@c.Image.FileId\" } }'></ul>\r\n}",
            "help": "",
            "links": ""
        },
        {
            "set": "@User",
            "subset": "Permissions",
            "name": "User is in role",
            "title": "",
            "content": "@Dnn.User.IsInRole(\"${1:Administrators}\")",
            "help": "Check if the current user is in a specific role",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Permissions",
            "name": "User is super user / host",
            "title": "",
            "content": "@Dnn.User.IsSuperUser",
            "help": "Check if the current user is the super-user aka host",
            "links": "read api-docs:https://github.com/2sic/2sxc/wiki/Razor-Dnn"
        },
        {
            "set": "@User",
            "subset": "Permissions",
            "name": "User is authenticated",
            "title": "",
            "content": "@if (Request.IsAuthenticated) {\r\n <!-- you're logged in -->\r\n}\r\nelse {\r\n  <!-- you're not logged in -->\r\n}",
            "help": "Check if the user is logged in.",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "using",
            "title": "",
            "content": "@using ${1:System.Linq}",
            "help": "",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "using 2sxc namespace",
            "title": "",
            "content": "@using ToSic.SexyContent;\r\n",
            "help": "The 2sxc namespace, in case you explicitly want to work with 2sxc objects",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "using Linq",
            "title": "",
            "content": "@using System.Linq;",
            "help": "",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "comment, server-side",
            "title": "",
            "content": "@* \r\n\r\nserver side multi-line comment\r\n\r\n*@",
            "help": "",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "functions block server-side",
            "title": "",
            "content": "@functions{\r\n\r\n}",
            "help": "",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "",
            "title": "",
            "content": "",
            "help": "",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "ASP.net Page",
            "name": "Set page title",
            "title": "",
            "content": "// set page title\r\nvar page = HttpContext.Current.Handler as Page;\r\npage.Title = \"${1:This page title works}\";",
            "help": "Set the page title of the dnn-page. Remember that this could happen multiple times, and the last one would always win.",
            "links": ""
        },
        {
            "set": "@C#",
            "subset": "ASP.net Page",
            "name": "Set header metadata",
            "title": "",
            "content": "// set MetaDescription - page variable must be declared before\r\nvar metaDescription = (HtmlMeta)page.FindControl(\"${2:metaDescription}\");\r\nmetaDescription.Content = \"${1:This MetaDescription works}\";",
            "help": "Set a meta-tag - in this case the Meta Description",
            "links": ""
        },
        {
            "set": "[Content",
            "subset": "General",
            "name": "Toolbar",
            "title": "",
            "content": "[${1:Content}:Toolbar]",
            "help": "Show an inline-toolbar. If you wat it hovering, make sure you have an HTML-element around it with the class sc-element",
            "links": ""
        },
        {
            "set": "[Content",
            "subset": "General",
            "name": "ToolbarFloat",
            "title": "",
            "content": "<div class=\"sc-element\">[${1:Content}:Toolbar]</div>",
            "help": "Show an inline toolbar, floating. Note that this is just an example, the div with the class sc-element can be further away, it doesn't have to be the direct container. ",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "Header",
            "name": "List-Toolbar",
            "title": "",
            "content": "[List:Toolbar]",
            "help": "",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "Header",
            "name": "List-ToolbarFloat",
            "title": "",
            "content": "<div class=\"sc-element\">[List:Toolbar]</div>",
            "help": "",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "Repeater",
            "name": "Repeater",
            "title": "",
            "content": "<repeat repeat=\"${1:Employee} in Data:${2:Default}\">...[${1}:Title]...</repeat>",
            "help": "Loop over a list of items in one of the input streams. The first value is the token-name inside the loop, the value after Data: is the stream name. Common use is Repeat=\"Item in Data:Default\"",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Count",
            "title": "",
            "content": "[${1:Employee}:Repeater:Count]",
            "help": "Get the count",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Index",
            "title": "",
            "content": "[${1:Employee}:Repeater:Index]",
            "help": "Get the index of the current item, zero based. So the number start with 0, 1, 2, …",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Index1",
            "title": "",
            "content": "[${1:Employee}:Repeater:Index1]",
            "help": "Get the index of the current item, one based. So the number start with 1, 2, 3, …",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "IsFirst",
            "title": "",
            "content": "[${1:Employee}:Repeater:IsFirst]",
            "help": "Find out if this is the first item",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "IsLast",
            "title": "",
            "content": "[${1:Employee}:Repeater:IsLast]",
            "help": "Find out if this is the last item",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator2",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator2]",
            "help": "Use this to alternate between 0, 1 - often used to color table rows",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator3",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator3]",
            "help": "Use this to alternate between 0, 1 and 2 - often used to have 3 items side-by side, followed by a new line",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator4",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator4]",
            "help": "",
            "links": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator5",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator5]",
            "help": "",
            "links": ""
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Path",
            "title": "",
            "content": "[App:Path]",
            "help": "path for integrating scripts,  images etc. For example  use as @App.Path/scripts/knockout.js",
            "links": ""
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "PhysicalPath",
            "title": "",
            "content": "[App:PhysicalPath]",
            "help": "physical path in c:\\",
            "links": ""
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Guid",
            "title": "",
            "content": "[App:AppGuid]",
            "help": "internal GUID - should stay the same across all systems for this specific App",
            "links": ""
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "AppId",
            "title": "",
            "content": "[App:AppId]",
            "help": "Id in the current data base. Is a different number in every App-Installation",
            "links": ""
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Name",
            "title": "",
            "content": "[App:Name]",
            "help": "internal name",
            "links": ""
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Folder",
            "title": "",
            "content": "[App:Folder]",
            "help": "often used to create paths to scripts or join some values. if you only need to reference a script,  please use App.Path",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Description",
            "title": "",
            "content": "[Module:Description]",
            "help": "Module Definition Description",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "EndDate",
            "title": "",
            "content": "[Module:EndDate]",
            "help": "Module Display Until Date",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Footer",
            "title": "",
            "content": "[Module:Footer]",
            "help": "Module Footer Text",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "FriendlyName",
            "title": "",
            "content": "[Module:FriendlyName]",
            "help": "Module Definition Name",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Header",
            "title": "",
            "content": "[Module:Header]",
            "help": "Module Header Text",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "HelpUrl",
            "title": "",
            "content": "[Module:HelpURL]",
            "help": "Module Help URL",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "IconFile",
            "title": "",
            "content": "[Module:IconFile]",
            "help": "Module Path to Icon File",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Title",
            "title": "",
            "content": "[Module:ModuleTitle]",
            "help": "Module Title",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "PaneName",
            "title": "",
            "content": "[Module:PaneName]",
            "help": "Module Name of Pane (where the module resides)",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "StartDate",
            "title": "",
            "content": "[Module:StartDate]",
            "help": "Module Display from Date",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "Description",
            "title": "",
            "content": "[Tab:Description]",
            "help": "Page Description Text for Search Engine",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "EndDate",
            "title": "",
            "content": "[Tab:EndDate]",
            "help": "Page Display Until Date",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "FullUrl",
            "title": "",
            "content": "[Tab:FullUrl]",
            "help": "Page Full URL",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "IconFile",
            "title": "",
            "content": "[Tab:IconFile]",
            "help": "Page Relative Path to Icon File",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "KeyWords",
            "title": "",
            "content": "[Tab:KeyWords]",
            "help": "Page Keywords for Search Engine",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "PageHeadtext",
            "title": "",
            "content": "[Tab:PageHeadText]",
            "help": "Page Header Text",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "StartDate",
            "title": "",
            "content": "[Tab:StartDate]",
            "help": "Page Display from Date",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "TabName",
            "title": "",
            "content": "[Tab:TabName]",
            "help": "Page Name",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "TabPath",
            "title": "",
            "content": "[Tab:TabPath]",
            "help": "Page Relative Path",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "Title",
            "title": "",
            "content": "[Tab:Title]",
            "help": "Page Title (Window Title)",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "Url",
            "title": "",
            "content": "[Tab:URL]",
            "help": "Page URL",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "Currency",
            "title": "",
            "content": "[Portal:Currency]",
            "help": "Currency String",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "Description",
            "title": "",
            "content": "[Portal:Description]",
            "help": "Portal Description",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "Email",
            "title": "",
            "content": "[Portal:Email]",
            "help": "Portal Admin Email",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "FooterText",
            "title": "",
            "content": "[Portal:FooterText]",
            "help": "Portal Copyright Text",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "HomeDirectory",
            "title": "",
            "content": "[Portal:HomeDirectory]",
            "help": "Portal Path (relative) of Home Directory",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "LogoFile",
            "title": "",
            "content": "[Portal:LogoFile]",
            "help": "Portal Path to Logo File",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "PortalName",
            "title": "",
            "content": "[Portal:PortalName]",
            "help": "Portal Name",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "PortalAlias",
            "title": "",
            "content": "[Portal:PortalAlias]",
            "help": "Portal URL",
            "links": ""
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "TimeZoneOffset",
            "title": "",
            "content": "[Portal:TimeZoneOffset]",
            "help": "Difference in Minutes between Portal Default Time and UTC",
            "links": ""
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "DisplayName",
            "title": "",
            "content": "[User:DisplayName]",
            "help": "User’s Display Name",
            "links": ""
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "Email",
            "title": "",
            "content": "[User:Email]",
            "help": "User’s Email Address",
            "links": ""
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "FirstName",
            "title": "",
            "content": "[User:FirstName]",
            "help": "User’s First Name",
            "links": ""
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "FullName",
            "title": "",
            "content": "[User:FullName]",
            "help": "(deprecated)",
            "links": ""
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "LastName",
            "title": "",
            "content": "[User:LastName]",
            "help": "User’s Last Name",
            "links": ""
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "UserName",
            "title": "",
            "content": "[User:Username]",
            "help": "User’s Login User Name",
            "links": ""
        },
        {
            "set": "[User",
            "subset": "Profile",
            "name": "Profile",
            "title": "",
            "content": "[Profile:${1:City}]",
            "help": "Use any default or custom Profile Property as listed in Profile Property Definition section of Manage User Accounts. Use non-localized Property Name only.",
            "links": ""
        },
        {
            "set": "[Environment",
            "subset": "QueryString",
            "name": "QueryString",
            "title": "",
            "content": "[QueryString:${1:ParameterName}]",
            "help": "Value of Querystring Name",
            "links": ""
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Now",
            "title": "",
            "content": "[DateTime:Now]",
            "help": "Current Date and Time",
            "links": ""
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Ticks-Now",
            "title": "",
            "content": "[Ticks:Now]",
            "help": "CPU Tick Count for Current Second",
            "links": ""
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Ticks-Today",
            "title": "",
            "content": "[Ticks:Today]",
            "help": "CPU Tick Count since Midnight",
            "links": ""
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Ticks-Per-Day",
            "title": "",
            "content": "[Ticks:TicksPerDay]",
            "help": "CPU Ticks per Day (for calculations)",
            "links": ""
        },
        {
            "set": "[Html",
            "subset": "Resources",
            "name": "script",
            "title": "",
            "content": "<script src=\"[App:Path]/dist/${1:myscripts}.js\" type=\"text/javascript\" data-enableoptimizations=\"100\"></script>",
            "help": "",
            "links": ""
        },
        {
            "set": "[Html",
            "subset": "Resources",
            "name": "css, style-sheet",
            "title": "",
            "content": "<link rel=\"stylesheet\" href=\"[App:Path]/dist/AppCatalog.css\" data-enableoptimizations=\"100\"/>",
            "help": "",
            "links": ""
        },
        {
            "set": "[\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail url",
            "title": "",
            "content": "[${101:var}:${102:prop}]?w=${1:200}&h=${2:200}&mode=${3:crop}",
            "help": "Thumbnail URL with crop-mode",
            "links": ""
        },
        {
            "set": "[\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail IMG tag",
            "title": "",
            "content": "<img src=\"[${101:var}:${102:prop}]?w=${1:200}&h=${2:200}&mode=${3:crop}\">",
            "help": "Thumbnail IMG tag with crop-mode",
            "links": ""
        },
        {
            "set": "@Html",
            "subset": "Resources",
            "name": "script",
            "title": "",
            "content": "<script src=\"@App.Path/dist/${1:myscripts}.js\" type=\"text/javascript\" data-enableoptimizations=\"100\"></script>",
            "help": "",
            "links": ""
        },
        {
            "set": "@Html",
            "subset": "Resources",
            "name": "css, style-sheet",
            "title": "",
            "content": "<link rel=\"stylesheet\" href=\"@App.Path/dist/AppCatalog.css\" data-enableoptimizations=\"100\"/>",
            "help": "",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-library",
            "name": "simple loop for assets",
            "title": "",
            "content": "@foreach(var ${3:pic} in AsAdam(${1:var}, \"${2:prop}\").Files){\r\n <span>@${3:pic}.Url, @${3:pic}.FileName </span>\r\n}",
            "help": "Adam: simple example with looping ADAM assets",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-library",
            "name": "loop with metadata assets",
            "title": "",
            "content": "@foreach(var ${3:pic} in AsAdam(${1:var}, \"${2:prop}\").Files){\r\n <div style=\"clear: both\">\r\n  <img src=\"@${3:pic}.Url?w=200&h=200&mode=crop\" title=\"@${3:pic}.FileName\" style=\"float: right\">\r\n  <h3>@${3:pic}.Metadata.${10:Title}</h3>\r\n  Has Meta: @${3:pic}.HasMetadata \r\n  <div>Description: @Html.Raw(${3:pic}.Metadata.${11:Description})</div>\r\n </div>\r\n}",
            "help": "Adam: Large example with looping ADAM assets",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-library",
            "name": "loop with type filter",
            "title": "",
            "content": "@foreach(var ${3:pic} in (AsAdam(${1:var}, \"${2:prop}\").Files as IEnumerable<ToSic.SexyContent.Adam.AdamFile>).Where(f => f.Type == \"${4:image}\")){\r\n <span>@${3:pic}.Url, @${3:pic}.FileName </span>\r\n}",
            "help": "",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail url",
            "title": "",
            "content": "@${101:var}.${102:prop}?w=${1:200}&h=${2:200}&mode=${3:crop}",
            "help": "Thumbnail URL with crop-mode",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail IMG tag",
            "title": "",
            "content": "<img src=\"@${101:var}.${102:prop}?w=${1:200}&h=${2:200}&mode=${3:crop}\">",
            "help": "Thumbnail IMG tag with crop-mode",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "string-wysiwyg",
            "name": "raw html",
            "title": "",
            "content": "@Html.Raw(${101:var}.${102:prop})",
            "help": "Output the html as html, not as text",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "string-url-path",
            "name": "link to url as parameter",
            "title": "",
            "content": "@Link.To(parameters: \"${10:id}=\" + ${1:var}.${2:prop})",
            "help": "Link to the same page but use this value as a url parameter",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "datetime",
            "name": "yyyy-MM-dd",
            "title": "",
            "content": "@${1:var}.${2:prop}.ToString(\"yyyy-MM-dd\")",
            "help": "format date with yyyy-MM-dd",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "content block with inpage editing",
            "title": "",
            "content": "<div class=\"sc-content-block-list\" @Edit.ContextAttributes(${101:var}, field: \"${102:prop}\")>\r\n    @foreach(var contentBlock in AsDynamic(${101:var}.${102:prop})){\r\n        @contentBlock.Render()\r\n    }\r\n</div>\r\n",
            "help": "Content blocks with in-page editing",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "render entity",
            "title": "",
            "content": "@${101:var}.render()",
            "help": "Render a content-block entity (remember that you can't render the list, this only renders one item).\r\nNote that if you try to render any kind of entity, this will not throw an error, but just render an HTML comment as there is no definition for how to render other types of entities as of now.",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "loop through items",
            "title": "",
            "content": "@foreach(var ${103:item} in AsDynamic(${101:var}.${102:prop})){\r\n    @${103:item}.EntityId\r\n}\r\n",
            "help": "loop over a list of sub-items",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "edit context",
            "title": "",
            "content": "@Edit.ContextAttributes(${101:var}, field: \"${102:prop}\")",
            "help": "This should be used inside a <div> tag to provide additional information to the in-page editing ui",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "item count",
            "title": "",
            "content": "@${101:var}.${102:prop}.Count",
            "help": "This will give you the count of items",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "get item number 0",
            "title": "",
            "content": "@${101:var}.${102:prop}[0]",
            "help": "Note that this only works, if there are items, otherwise you'll get an error",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "get first item",
            "title": "",
            "content": "@${101:var}.${102:prop}[0]",
            "help": "This also requires a @using Tosic.SexyContent; at the beginning of your cshtml",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "show title of first item if exists",
            "title": "",
            "content": "@(${1:var}.${2:prop}.Count > 0 ? ${1:var}.${2:prop}[0].Title : \"\")",
            "help": "This is a shorthand for try-to-get-and-if-null-don’t-show",
            "links": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "render first if it exists",
            "title": "",
            "content": "@(${1:var}.${2:prop}.Count > 0 ? ${1:var}.${2:prop}[0].Render() : \"\")",
            "help": "This is a shorthand for try-to-render-first-and-if-null-don’t-show",
            "links": ""
        }
    ]
}