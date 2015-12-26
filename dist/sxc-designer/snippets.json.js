{
    "snippets": [
        {
            "set": "@DnnRazor",
            "subset": "App",
            "key": "Path",
            "title": "returns the url to the current app",
            "tabTrigger": "path",
            "content": "@App.Path",
            "help": "path for integrating scripts,  images etc. For example  use as @App.Path/scripts/knockout.js"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "key": "PhysPath",
            "title": "physical path",
            "tabTrigger": "physical path",
            "content": "@App.PhysicalPath",
            "help": "physical path in c:\\"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "key": "AppGuid",
            "title": "App Guid",
            "tabTrigger": "app guid",
            "content": "@App.AppGuid",
            "help": "internal GUID - should stay the same across all systems for this specific App"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "key": "AppId",
            "title": "App Id",
            "tabTrigger": "app id",
            "content": "@App.AppId",
            "help": "Id in the current data base. Is a different number in every App-Installation"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "key": "AppName",
            "title": "App Name",
            "tabTrigger": "app name",
            "content": "@App.Name",
            "help": "internal name"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "key": "AppFolder",
            "title": "folder of the 2sxc-app",
            "tabTrigger": "app folder",
            "content": "@App.Folder",
            "help": "often used to create paths to scripts or join some values. if you only need to reference a script,  please use App.Path"
        },
        {
            "set": "@DnnRazor",
            "subset": "Content",
            "key": "@Toolbar",
            "title": "",
            "tabTrigger": "",
            "content": "@${1:Content}.Toolbar",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Content",
            "key": "@ToolbarFloat",
            "title": "",
            "tabTrigger": "",
            "content": "<div class=\\sc-element\\>@${1:Content}.Toolbar</div>",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "List",
            "key": "@Toolbar",
            "title": "Header toolbar",
            "tabTrigger": "",
            "content": "@List.Toolbar",
            "help": "Outputs the toolbar to edit list information - place in a <div> to float like other toolbars"
        },
        {
            "set": "@DnnRazor",
            "subset": "List",
            "key": "@ToolbarFloat",
            "title": "",
            "tabTrigger": "",
            "content": "<div class=\\sc-element\\>@List.Toolbar</div>",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "AllModules",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.AllModules",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "AllTabs",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.AllTabs",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "Border",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.Border",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "Cacheability",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.Cacheability",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "CacheMethod",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.CacheMethod",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "CacheTime",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.CacheTime",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ContainerPath",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ContainerPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ContainerSrc",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ContainerSrc",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "DefaultCacheTime",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.DefaultCacheTime",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "DesktopModule",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.DesktopModule",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "DesktopModuleId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.DesktopModuleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "DisplayTitle",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.DisplayTitle",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "EndDate",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.EndDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "Footer",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.Footer",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "Header",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.Header",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleControl",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleControl",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleControlId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleControlId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleDefID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleDefID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleDefinition",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleDefinition",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleOrder",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleOrder",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModulePermissions",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModulePermissions",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleSettings",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleSettings",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ModuleTitle",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ModuleTitle",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "PaneModuleCount",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.PaneModuleCount",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "PaneModuleIndex",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.PaneModuleIndex",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "PaneName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.PaneName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "ParentTab",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.ParentTab",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "PortalID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.PortalID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "StartDate",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.StartDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "TabID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.TabID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "TabModuleID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.TabModuleID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "TabModuleSettings",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.TabModuleSettings",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "UniqueId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.UniqueId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "key": "Visibility",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Module.Visibility",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "ChildModules",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.ChildModules",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "ContainerPath",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.ContainerPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "ContainerSrc",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.ContainerSrc",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "CultureCode",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.CultureCode",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "DefaultLanguageGuid",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.DefaultLanguageGuid",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "DefaultLanguageTab",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.DefaultLanguageTab",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "Description",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.Description",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "EndDate",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.EndDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "FullUrl",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.FullUrl",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "HasChildren",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.HasChildren",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "IsDefaultLanguage",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.IsDefaultLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "IsDeleted",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.IsDeleted",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "IsNeutralCulture",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.IsNeutralCulture",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "IsSecure",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.IsSecure",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "IsTranslated",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.IsTranslated",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "IsVisible",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.IsVisible",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "KeyWords",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.KeyWords",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "Level",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.Level",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "LocalizedTabName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.LocalizedTabName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "LocalizedTabs",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.LocalizedTabs",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "Modules",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.Modules",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "PageHeadtext",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.PageHeadtext",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "Panes",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.Panes",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "ParentId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.ParentId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "PermanentRedirect",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.PermanentRedirect",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "PortalID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.PortalID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "RefreshInterval",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.RefreshInterval",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "SiteMapPriority",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.SiteMapPriority",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "SkinDoctype",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.SkinDoctype",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "SkinPath",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.SkinPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "SkinSrc",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.SkinSrc",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "StartDate",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.StartDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "TabID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.TabID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "TabName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.TabName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "TabOrder",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.TabOrder",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "TabPath",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.TabPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "TabPermissions",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.TabPermissions",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "TabSettings",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.TabSettings",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "TabType",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.TabType",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "Title",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.Title",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "UniqueId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.UniqueId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "key": "Url",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Tab.Url",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "ActiveTab",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.ActiveTab",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "AdministratorId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.AdministratorId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "AdministratorRoleId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.AdministratorRoleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "AdministratoRoleName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.AdministratoRoleName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "AdminTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.AdminTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "CultureCode",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.CultureCode",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Currency",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Currency",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Current",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Current",
            "help": "static method returning the current portal portal-settings"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Default",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Default",
            "help": "(AdminContainer, AdminSkin, ControlPanelMode, etc.)"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "DefaultLanguage",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.DefaultLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "DefaultModuleId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.DefaultModuleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "DefaultPortalContainer",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.DefaultPortalContainer",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "DefaultTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.DefaultTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Description",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Description",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Email",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Email",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "EnableBrowserLanguage",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.EnableBrowserLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "EnableUrlLanguage",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.EnableUrlLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "ExpiryDate",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.ExpiryDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "FooterText",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.FooterText",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Guid",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.GUID - notice all large letters",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "HomeDirectory",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.HomeDirectory",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "HomeDirectoryMapPath",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.HomeDirectoryMapPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "HomeTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.HomeTabId",
            "help": "the root page"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "KeyWords",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.KeyWords",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "LoginTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.LoginTabId",
            "help": "where the normal login usually goes to"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "LogoFile",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.LogoFile",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Pages",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Pages",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "PortalAlias",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.PortalAlias",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "PortalId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.PortalId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "PortalName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.PortalName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "PortalSkin",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.PortalSkin",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "RegisteredRoleId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.RegisteredRoleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "RegisteredRoleName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.RegisteredRoleName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "RegisterTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.RegisterTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Search",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Search.something",
            "help": "various values like IncludeCommon, IncludeTagInfoFilter etc."
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "SearchTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.SearchTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "SiteLogHistory",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.SiteLogHistory",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "SplashTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.SplashTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "SSLEnabled",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.SSLEnabled",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "SSLEnforced",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.SSLEnforced",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "SSLURL",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.SSLURL",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "STDURL",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.STDURL",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "SuperTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.SuperTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "TimeZoneOffset",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.TimeZoneOffset",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "UserId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.UserId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "UserInfo",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.UserInfo",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "UserMode",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.UserMode",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "UserQuota",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.UserQuota",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "UserRegistration",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.UserRegistration",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "Users",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.Users",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "key": "UserTabId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.Portal.UserTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "DisplayName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.DisplayName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "Email",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.Email",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "FirstName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.FirstName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "FullName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.FullName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "IsDeleted",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.IsDeleted",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "IsInRole",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.IsInRole(string RoleName)",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "IsSuperUser",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.IsSuperUser",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "LastName",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.LastName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "Membership",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.Membership",
            "help": "these are asp.net memberships, you probably don't need them"
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "PortalId",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.PortalId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "Roles",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.Roles",
            "help": "these are DNN roles"
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "UserID",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.UserID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "key": "Username",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.Username",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Profile",
            "key": "AnyValue",
            "title": "",
            "tabTrigger": "",
            "content": "@Dnn.User.Profile.GetPropertyValue(\\${1:City}\\)",
            "help": "any property of the user profile as configured in your portal"
        }
    ]
}