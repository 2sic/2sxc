{
    "snippets": [
        {
            "set": "@DnnRazor",
            "subset": "App",
            "name": "Path",
            "title": "returns the url to the current app",
            "content": "@App.Path",
            "help": "path for integrating scripts,  images etc. For example  use as @App.Path/scripts/knockout.js"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "name": "PhysPath",
            "title": "physical path",
            "content": "@App.PhysicalPath",
            "help": "physical path in c:\\"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "name": "AppGuid",
            "title": "App Guid",
            "content": "@App.AppGuid",
            "help": "internal GUID - should stay the same across all systems for this specific App"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "name": "AppId",
            "title": "App Id",
            "content": "@App.AppId",
            "help": "Id in the current data base. Is a different number in every App-Installation"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "name": "AppName",
            "title": "App Name",
            "content": "@App.Name",
            "help": "internal name"
        },
        {
            "set": "@DnnRazor",
            "subset": "App",
            "name": "AppFolder",
            "title": "folder of the 2sxc-app",
            "content": "@App.Folder",
            "help": "often used to create paths to scripts or join some values. if you only need to reference a script,  please use App.Path"
        },
        {
            "set": "@DnnRazor",
            "subset": "Content",
            "name": "@Toolbar",
            "title": "",
            "content": "@${1:Content}.Toolbar",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Content",
            "name": "@ToolbarFloat",
            "title": "",
            "content": "<div class=\\sc-element\\>@${1:Content}.Toolbar</div>",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "List",
            "name": "@Toolbar",
            "title": "Header toolbar",
            "content": "@List.Toolbar",
            "help": "Outputs the toolbar to edit list information - place in a <div> to float like other toolbars"
        },
        {
            "set": "@DnnRazor",
            "subset": "List",
            "name": "@ToolbarFloat",
            "title": "",
            "content": "<div class=\\sc-element\\>@List.Toolbar</div>",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "AllModules",
            "title": "",
            "content": "@Dnn.Module.AllModules",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "AllTabs",
            "title": "",
            "content": "@Dnn.Module.AllTabs",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Border",
            "title": "",
            "content": "@Dnn.Module.Border",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Cacheability",
            "title": "",
            "content": "@Dnn.Module.Cacheability",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "CacheMethod",
            "title": "",
            "content": "@Dnn.Module.CacheMethod",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "CacheTime",
            "title": "",
            "content": "@Dnn.Module.CacheTime",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ContainerPath",
            "title": "",
            "content": "@Dnn.Module.ContainerPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ContainerSrc",
            "title": "",
            "content": "@Dnn.Module.ContainerSrc",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DefaultCacheTime",
            "title": "",
            "content": "@Dnn.Module.DefaultCacheTime",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DesktopModule",
            "title": "",
            "content": "@Dnn.Module.DesktopModule",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DesktopModuleId",
            "title": "",
            "content": "@Dnn.Module.DesktopModuleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "DisplayTitle",
            "title": "",
            "content": "@Dnn.Module.DisplayTitle",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "EndDate",
            "title": "",
            "content": "@Dnn.Module.EndDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Footer",
            "title": "",
            "content": "@Dnn.Module.Footer",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Header",
            "title": "",
            "content": "@Dnn.Module.Header",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleControl",
            "title": "",
            "content": "@Dnn.Module.ModuleControl",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleControlId",
            "title": "",
            "content": "@Dnn.Module.ModuleControlId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleDefID",
            "title": "",
            "content": "@Dnn.Module.ModuleDefID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleDefinition",
            "title": "",
            "content": "@Dnn.Module.ModuleDefinition",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleID",
            "title": "",
            "content": "@Dnn.Module.ModuleID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleName",
            "title": "",
            "content": "@Dnn.Module.ModuleName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleOrder",
            "title": "",
            "content": "@Dnn.Module.ModuleOrder",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModulePermissions",
            "title": "",
            "content": "@Dnn.Module.ModulePermissions",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleSettings",
            "title": "",
            "content": "@Dnn.Module.ModuleSettings",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ModuleTitle",
            "title": "",
            "content": "@Dnn.Module.ModuleTitle",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PaneModuleCount",
            "title": "",
            "content": "@Dnn.Module.PaneModuleCount",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PaneModuleIndex",
            "title": "",
            "content": "@Dnn.Module.PaneModuleIndex",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PaneName",
            "title": "",
            "content": "@Dnn.Module.PaneName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "ParentTab",
            "title": "",
            "content": "@Dnn.Module.ParentTab",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "PortalID",
            "title": "",
            "content": "@Dnn.Module.PortalID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "StartDate",
            "title": "",
            "content": "@Dnn.Module.StartDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "TabID",
            "title": "",
            "content": "@Dnn.Module.TabID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "TabModuleID",
            "title": "",
            "content": "@Dnn.Module.TabModuleID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "TabModuleSettings",
            "title": "",
            "content": "@Dnn.Module.TabModuleSettings",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "UniqueId",
            "title": "",
            "content": "@Dnn.Module.UniqueId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Module",
            "name": "Visibility",
            "title": "",
            "content": "@Dnn.Module.Visibility",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ChildModules",
            "title": "",
            "content": "@Dnn.Tab.ChildModules",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ContainerPath",
            "title": "",
            "content": "@Dnn.Tab.ContainerPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ContainerSrc",
            "title": "",
            "content": "@Dnn.Tab.ContainerSrc",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "CultureCode",
            "title": "",
            "content": "@Dnn.Tab.CultureCode",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "DefaultLanguageGuid",
            "title": "",
            "content": "@Dnn.Tab.DefaultLanguageGuid",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "DefaultLanguageTab",
            "title": "",
            "content": "@Dnn.Tab.DefaultLanguageTab",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Description",
            "title": "",
            "content": "@Dnn.Tab.Description",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "EndDate",
            "title": "",
            "content": "@Dnn.Tab.EndDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "FullUrl",
            "title": "",
            "content": "@Dnn.Tab.FullUrl",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "HasChildren",
            "title": "",
            "content": "@Dnn.Tab.HasChildren",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsDefaultLanguage",
            "title": "",
            "content": "@Dnn.Tab.IsDefaultLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsDeleted",
            "title": "",
            "content": "@Dnn.Tab.IsDeleted",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsNeutralCulture",
            "title": "",
            "content": "@Dnn.Tab.IsNeutralCulture",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsSecure",
            "title": "",
            "content": "@Dnn.Tab.IsSecure",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsTranslated",
            "title": "",
            "content": "@Dnn.Tab.IsTranslated",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "IsVisible",
            "title": "",
            "content": "@Dnn.Tab.IsVisible",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "KeyWords",
            "title": "",
            "content": "@Dnn.Tab.KeyWords",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Level",
            "title": "",
            "content": "@Dnn.Tab.Level",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "LocalizedTabName",
            "title": "",
            "content": "@Dnn.Tab.LocalizedTabName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "LocalizedTabs",
            "title": "",
            "content": "@Dnn.Tab.LocalizedTabs",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Modules",
            "title": "",
            "content": "@Dnn.Tab.Modules",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "PageHeadtext",
            "title": "",
            "content": "@Dnn.Tab.PageHeadtext",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Panes",
            "title": "",
            "content": "@Dnn.Tab.Panes",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "ParentId",
            "title": "",
            "content": "@Dnn.Tab.ParentId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "PermanentRedirect",
            "title": "",
            "content": "@Dnn.Tab.PermanentRedirect",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "PortalID",
            "title": "",
            "content": "@Dnn.Tab.PortalID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "RefreshInterval",
            "title": "",
            "content": "@Dnn.Tab.RefreshInterval",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SiteMapPriority",
            "title": "",
            "content": "@Dnn.Tab.SiteMapPriority",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SkinDoctype",
            "title": "",
            "content": "@Dnn.Tab.SkinDoctype",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SkinPath",
            "title": "",
            "content": "@Dnn.Tab.SkinPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "SkinSrc",
            "title": "",
            "content": "@Dnn.Tab.SkinSrc",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "StartDate",
            "title": "",
            "content": "@Dnn.Tab.StartDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabID",
            "title": "",
            "content": "@Dnn.Tab.TabID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabName",
            "title": "",
            "content": "@Dnn.Tab.TabName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabOrder",
            "title": "",
            "content": "@Dnn.Tab.TabOrder",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabPath",
            "title": "",
            "content": "@Dnn.Tab.TabPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabPermissions",
            "title": "",
            "content": "@Dnn.Tab.TabPermissions",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabSettings",
            "title": "",
            "content": "@Dnn.Tab.TabSettings",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "TabType",
            "title": "",
            "content": "@Dnn.Tab.TabType",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Title",
            "title": "",
            "content": "@Dnn.Tab.Title",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "UniqueId",
            "title": "",
            "content": "@Dnn.Tab.UniqueId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Tab",
            "name": "Url",
            "title": "",
            "content": "@Dnn.Tab.Url",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "ActiveTab",
            "title": "",
            "content": "@Dnn.Portal.ActiveTab",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdministratorId",
            "title": "",
            "content": "@Dnn.Portal.AdministratorId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdministratorRoleId",
            "title": "",
            "content": "@Dnn.Portal.AdministratorRoleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdministratoRoleName",
            "title": "",
            "content": "@Dnn.Portal.AdministratoRoleName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "AdminTabId",
            "title": "",
            "content": "@Dnn.Portal.AdminTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "CultureCode",
            "title": "",
            "content": "@Dnn.Portal.CultureCode",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Currency",
            "title": "",
            "content": "@Dnn.Portal.Currency",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Current",
            "title": "",
            "content": "@Dnn.Portal.Current",
            "help": "static method returning the current portal portal-settings"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Default",
            "title": "",
            "content": "@Dnn.Portal.Default",
            "help": "(AdminContainer, AdminSkin, ControlPanelMode, etc.)"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultLanguage",
            "title": "",
            "content": "@Dnn.Portal.DefaultLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultModuleId",
            "title": "",
            "content": "@Dnn.Portal.DefaultModuleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultPortalContainer",
            "title": "",
            "content": "@Dnn.Portal.DefaultPortalContainer",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "DefaultTabId",
            "title": "",
            "content": "@Dnn.Portal.DefaultTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Description",
            "title": "",
            "content": "@Dnn.Portal.Description",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Email",
            "title": "",
            "content": "@Dnn.Portal.Email",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "EnableBrowserLanguage",
            "title": "",
            "content": "@Dnn.Portal.EnableBrowserLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "EnableUrlLanguage",
            "title": "",
            "content": "@Dnn.Portal.EnableUrlLanguage",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "ExpiryDate",
            "title": "",
            "content": "@Dnn.Portal.ExpiryDate",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "FooterText",
            "title": "",
            "content": "@Dnn.Portal.FooterText",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Guid",
            "title": "",
            "content": "@Dnn.Portal.GUID - notice all large letters",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "HomeDirectory",
            "title": "",
            "content": "@Dnn.Portal.HomeDirectory",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "HomeDirectoryMapPath",
            "title": "",
            "content": "@Dnn.Portal.HomeDirectoryMapPath",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "HomeTabId",
            "title": "",
            "content": "@Dnn.Portal.HomeTabId",
            "help": "the root page"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "KeyWords",
            "title": "",
            "content": "@Dnn.Portal.KeyWords",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "LoginTabId",
            "title": "",
            "content": "@Dnn.Portal.LoginTabId",
            "help": "where the normal login usually goes to"
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "LogoFile",
            "title": "",
            "content": "@Dnn.Portal.LogoFile",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Pages",
            "title": "",
            "content": "@Dnn.Portal.Pages",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalAlias",
            "title": "",
            "content": "@Dnn.Portal.PortalAlias",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalId",
            "title": "",
            "content": "@Dnn.Portal.PortalId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalName",
            "title": "",
            "content": "@Dnn.Portal.PortalName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "PortalSkin",
            "title": "",
            "content": "@Dnn.Portal.PortalSkin",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "RegisteredRoleId",
            "title": "",
            "content": "@Dnn.Portal.RegisteredRoleId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "RegisteredRoleName",
            "title": "",
            "content": "@Dnn.Portal.RegisteredRoleName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "RegisterTabId",
            "title": "",
            "content": "@Dnn.Portal.RegisterTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Search",
            "title": "",
            "content": "@Dnn.Portal.Search.something",
            "help": "various values like IncludeCommon, IncludeTagInfoFilter etc."
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SearchTabId",
            "title": "",
            "content": "@Dnn.Portal.SearchTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SiteLogHistory",
            "title": "",
            "content": "@Dnn.Portal.SiteLogHistory",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SplashTabId",
            "title": "",
            "content": "@Dnn.Portal.SplashTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SSLEnabled",
            "title": "",
            "content": "@Dnn.Portal.SSLEnabled",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SSLEnforced",
            "title": "",
            "content": "@Dnn.Portal.SSLEnforced",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SSLURL",
            "title": "",
            "content": "@Dnn.Portal.SSLURL",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "STDURL",
            "title": "",
            "content": "@Dnn.Portal.STDURL",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "SuperTabId",
            "title": "",
            "content": "@Dnn.Portal.SuperTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "TimeZoneOffset",
            "title": "",
            "content": "@Dnn.Portal.TimeZoneOffset",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserId",
            "title": "",
            "content": "@Dnn.Portal.UserId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserInfo",
            "title": "",
            "content": "@Dnn.Portal.UserInfo",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserMode",
            "title": "",
            "content": "@Dnn.Portal.UserMode",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserQuota",
            "title": "",
            "content": "@Dnn.Portal.UserQuota",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserRegistration",
            "title": "",
            "content": "@Dnn.Portal.UserRegistration",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "Users",
            "title": "",
            "content": "@Dnn.Portal.Users",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Portal",
            "name": "UserTabId",
            "title": "",
            "content": "@Dnn.Portal.UserTabId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "DisplayName",
            "title": "",
            "content": "@Dnn.User.DisplayName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "Email",
            "title": "",
            "content": "@Dnn.User.Email",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "FirstName",
            "title": "",
            "content": "@Dnn.User.FirstName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "FullName",
            "title": "",
            "content": "@Dnn.User.FullName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "IsDeleted",
            "title": "",
            "content": "@Dnn.User.IsDeleted",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "IsInRole",
            "title": "",
            "content": "@Dnn.User.IsInRole(string RoleName)",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "IsSuperUser",
            "title": "",
            "content": "@Dnn.User.IsSuperUser",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "LastName",
            "title": "",
            "content": "@Dnn.User.LastName",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "Membership",
            "title": "",
            "content": "@Dnn.User.Membership",
            "help": "these are asp.net memberships, you probably don't need them"
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "PortalId",
            "title": "",
            "content": "@Dnn.User.PortalId",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "Roles",
            "title": "",
            "content": "@Dnn.User.Roles",
            "help": "these are DNN roles"
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "UserID",
            "title": "",
            "content": "@Dnn.User.UserID",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "User",
            "name": "Username",
            "title": "",
            "content": "@Dnn.User.Username",
            "help": ""
        },
        {
            "set": "@DnnRazor",
            "subset": "Profile",
            "name": "AnyValue",
            "title": "",
            "content": "@Dnn.User.Profile.GetPropertyValue(\\${1:City}\\)",
            "help": "any property of the user profile as configured in your portal"
        }
    ]
}