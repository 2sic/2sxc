{
    "snippets": [
        {
            "set": "@App",
            "subset": "App",
            "name": "Path",
            "title": "returns the url to the current app",
            "content": "@App.Path",
            "help": "path for integrating scripts,  images etc. For example  use as @App.Path/scripts/knockout.js"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "PhysPath",
            "title": "physical path",
            "content": "@App.PhysicalPath",
            "help": "physical path in c:\\"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppGuid",
            "title": "App Guid",
            "content": "@App.AppGuid",
            "help": "internal GUID - should stay the same across all systems for this specific App"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppId",
            "title": "App Id",
            "content": "@App.AppId",
            "help": "Id in the current data base. Is a different number in every App-Installation"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppName",
            "title": "App Name",
            "content": "@App.Name",
            "help": "internal name"
        },
        {
            "set": "@App",
            "subset": "App",
            "name": "AppFolder",
            "title": "folder of the 2sxc-app",
            "content": "@App.Folder",
            "help": "often used to create paths to scripts or join some values. if you only need to reference a script,  please use App.Path"
        },
        {
            "set": "@Content",
            "subset": "General",
            "name": "Toolbar",
            "title": "",
            "content": "@${1:Content}.Toolbar",
            "help": ""
        },
        {
            "set": "@Content",
            "subset": "General",
            "name": "ToolbarFloat",
            "title": "",
            "content": "<div class=\"sc-element\">\r\n    @${1:Content}.Toolbar\r\n</div>",
            "help": ""
        },
        {
            "set": "@List",
            "subset": "Header",
            "name": "List-Toolbar",
            "title": "Header toolbar",
            "content": "@List.Toolbar",
            "help": "Outputs the toolbar to edit list information - place in a <div> to float like other toolbars"
        },
        {
            "set": "@List",
            "subset": "Header",
            "name": "List-ToolbarFloat",
            "title": "",
            "content": "<div class=\"sc-element\">\r\n    @List.Toolbar\r\n</div>",
            "help": ""
        },
        {
            "set": "@List",
            "subset": "Repeater",
            "name": "foreach loop",
            "title": "",
            "content": "@foreach(var ${1:cont} in AsDynamic(Data[\"${2:Default}\"])){\r\n    <div class=\"sc-element\">\r\n        @${1}.EntityTitle\r\n        @${1}.Toolbar\r\n    </div>\r\n}…",
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
            "help": "for automatic hiding of the page"
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
            "set": "@User",
            "subset": "Basic",
            "name": "DisplayName",
            "title": "",
            "content": "@Dnn.User.DisplayName",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Email",
            "title": "",
            "content": "@Dnn.User.Email",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "FirstName",
            "title": "",
            "content": "@Dnn.User.FirstName",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "FullName",
            "title": "",
            "content": "@Dnn.User.FullName",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "IsDeleted",
            "title": "",
            "content": "@Dnn.User.IsDeleted",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "IsInRole",
            "title": "",
            "content": "@Dnn.User.IsInRole(string RoleName)",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "IsSuperUser",
            "title": "",
            "content": "@Dnn.User.IsSuperUser",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "LastName",
            "title": "",
            "content": "@Dnn.User.LastName",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Membership",
            "title": "",
            "content": "@Dnn.User.Membership",
            "help": "these are asp.net memberships, you probably don't need them"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "PortalId",
            "title": "",
            "content": "@Dnn.User.PortalId",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Roles",
            "title": "",
            "content": "@Dnn.User.Roles",
            "help": "these are DNN roles"
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "UserID",
            "title": "",
            "content": "@Dnn.User.UserID",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Basic",
            "name": "Username",
            "title": "",
            "content": "@Dnn.User.Username",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Profile",
            "name": "AnyValue",
            "title": "",
            "content": "@Dnn.User.Profile.GetPropertyValue(\\${1:City}\\)",
            "help": "any property of the user profile as configured in your portal"
        },
        {
            "set": "@User",
            "subset": "Permissions",
            "name": "IsEditMode",
            "title": "",
            "content": "@if (DotNetNuke.Common.Globals.IsEditMode())\r\n{\r\n        <div> stuff here which only appears in edit mode </div>\r\n}",
            "help": ""
        },
        {
            "set": "@User",
            "subset": "Permissions",
            "name": "UserMayEditContent",
            "title": "",
            "content": "@if(Permissions.UserMayEditContent)\r\n{\r\n       <!-- stuff, like a custom edit toolbar - here an example -->\r\n       <ul class=\"sc-menu\" data-toolbar='{ \"action\": \"new\", \"contentType\": \"Gallery Image\", \"prefill\": { \"File\": \"File:@c.Image.FileId\" } }'></ul>\r\n}",
            "help": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "using",
            "title": "",
            "content": "@using ${1:System.Linq}",
            "help": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "using 2sxc namespace",
            "title": "",
            "content": "@using ToSic.SexyContent;\r\n",
            "help": "The 2sxc namespace, in case you explicitly want to work with 2sxc objects"
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "using Linq",
            "title": "",
            "content": "@using System.Linq;",
            "help": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "comment, server-side",
            "title": "",
            "content": "@* \r\n\r\nserver side multi-line comment\r\n\r\n*@",
            "help": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "functions block server-side",
            "title": "",
            "content": "@functions{\r\n\r\n}",
            "help": ""
        },
        {
            "set": "@C#",
            "subset": "Basics",
            "name": "",
            "title": "",
            "content": "",
            "help": ""
        },
        {
            "set": "[Content",
            "subset": "General",
            "name": "Toolbar",
            "title": "",
            "content": "[${1:Content}:Toolbar]",
            "help": ""
        },
        {
            "set": "[Content",
            "subset": "General",
            "name": "ToolbarFloat",
            "title": "",
            "content": "<div class=\\sc-element\\>[${1:Content}:Toolbar]</div>",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "Header",
            "name": "List-Toolbar",
            "title": "",
            "content": "[List:Toolbar]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "Header",
            "name": "List-ToolbarFloat",
            "title": "",
            "content": "<div class=\\sc-element\\>[List:Toolbar]</div>",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "Repeater",
            "name": "Repeater",
            "title": "",
            "content": "<repeat repeat=\\${1:Employee} in Data:${2:Default}\\>...[${1}:Title]...</repeat>",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Count",
            "title": "",
            "content": "[${1:Employee}:Repeater:Count]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Index",
            "title": "",
            "content": "[${1:Employee}:Repeater:Index]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Index1",
            "title": "",
            "content": "[${1:Employee}:Repeater:Index1]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "IsFirst",
            "title": "",
            "content": "[${1:Employee}:Repeater:IsFirst]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "IsLast",
            "title": "",
            "content": "[${1:Employee}:Repeater:IsLast]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator2",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator2]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator3",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator3]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator4",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator4]",
            "help": ""
        },
        {
            "set": "[List",
            "subset": "LoopItems",
            "name": "Alternator5",
            "title": "",
            "content": "[${1:Employee}:Repeater:Alternator5]",
            "help": ""
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Path",
            "title": "",
            "content": "[App:Path]",
            "help": "path for integrating scripts,  images etc. For example  use as @App.Path/scripts/knockout.js"
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "PhysicalPath",
            "title": "",
            "content": "[App:PhysicalPath]",
            "help": "physical path in c:\\"
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Guid",
            "title": "",
            "content": "[App:AppGuid]",
            "help": "internal GUID - should stay the same across all systems for this specific App"
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "AppId",
            "title": "",
            "content": "[App:AppId]",
            "help": "Id in the current data base. Is a different number in every App-Installation"
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Name",
            "title": "",
            "content": "[App:Name]",
            "help": "internal name"
        },
        {
            "set": "[App",
            "subset": "App",
            "name": "Folder",
            "title": "",
            "content": "[App:Folder]",
            "help": "often used to create paths to scripts or join some values. if you only need to reference a script,  please use App.Path"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Description",
            "title": "",
            "content": "[Module:Description]",
            "help": "Module Definition Description"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "EndDate",
            "title": "",
            "content": "[Module:EndDate]",
            "help": "Module Display Until Date"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Footer",
            "title": "",
            "content": "[Module:Footer]",
            "help": "Module Footer Text"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "FriendlyName",
            "title": "",
            "content": "[Module:FriendlyName]",
            "help": "Module Definition Name"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Header",
            "title": "",
            "content": "[Module:Header]",
            "help": "Module Header Text"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "HelpUrl",
            "title": "",
            "content": "[Module:HelpURL]",
            "help": "Module Help URL"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "IconFile",
            "title": "",
            "content": "[Module:IconFile]",
            "help": "Module Path to Icon File"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "Title",
            "title": "",
            "content": "[Module:ModuleTitle]",
            "help": "Module Title"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "PaneName",
            "title": "",
            "content": "[Module:PaneName]",
            "help": "Module Name of Pane (where the module resides)"
        },
        {
            "set": "[DnnToken",
            "subset": "Module",
            "name": "StartDate",
            "title": "",
            "content": "[Module:StartDate]",
            "help": "Module Display from Date"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "Description",
            "title": "",
            "content": "[Tab:Description]",
            "help": "Page Description Text for Search Engine"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "EndDate",
            "title": "",
            "content": "[Tab:EndDate]",
            "help": "Page Display Until Date"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "FullUrl",
            "title": "",
            "content": "[Tab:FullUrl]",
            "help": "Page Full URL"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "IconFile",
            "title": "",
            "content": "[Tab:IconFile]",
            "help": "Page Relative Path to Icon File"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "KeyWords",
            "title": "",
            "content": "[Tab:KeyWords]",
            "help": "Page Keywords for Search Engine"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "PageHeadtext",
            "title": "",
            "content": "[Tab:PageHeadText]",
            "help": "Page Header Text"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "StartDate",
            "title": "",
            "content": "[Tab:StartDate]",
            "help": "Page Display from Date"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "TabName",
            "title": "",
            "content": "[Tab:TabName]",
            "help": "Page Name"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "TabPath",
            "title": "",
            "content": "[Tab:TabPath]",
            "help": "Page Relative Path"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "Title",
            "title": "",
            "content": "[Tab:Title]",
            "help": "Page Title (Window Title)"
        },
        {
            "set": "[DnnToken",
            "subset": "Tab",
            "name": "Url",
            "title": "",
            "content": "[Tab:URL]",
            "help": "Page URL"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "Currency",
            "title": "",
            "content": "[Portal:Currency]",
            "help": "Currency String"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "Description",
            "title": "",
            "content": "[Portal:Description]",
            "help": "Portal Description"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "Email",
            "title": "",
            "content": "[Portal:Email]",
            "help": "Portal Admin Email"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "FooterText",
            "title": "",
            "content": "[Portal:FooterText]",
            "help": "Portal Copyright Text"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "HomeDirectory",
            "title": "",
            "content": "[Portal:HomeDirectory]",
            "help": "Portal Path (relative) of Home Directory"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "LogoFile",
            "title": "",
            "content": "[Portal:LogoFile]",
            "help": "Portal Path to Logo File"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "PortalName",
            "title": "",
            "content": "[Portal:PortalName]",
            "help": "Portal Name"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "PortalAlias",
            "title": "",
            "content": "[Portal:PortalAlias]",
            "help": "Portal URL"
        },
        {
            "set": "[DnnToken",
            "subset": "Portal",
            "name": "TimeZoneOffset",
            "title": "",
            "content": "[Portal:TimeZoneOffset]",
            "help": "Difference in Minutes between Portal Default Time and UTC"
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "DisplayName",
            "title": "",
            "content": "[User:DisplayName]",
            "help": "User’s Display Name"
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "Email",
            "title": "",
            "content": "[User:Email]",
            "help": "User’s Email Address"
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "FirstName",
            "title": "",
            "content": "[User:FirstName]",
            "help": "User’s First Name"
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "FullName",
            "title": "",
            "content": "[User:FullName]",
            "help": "(deprecated)"
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "LastName",
            "title": "",
            "content": "[User:LastName]",
            "help": "User’s Last Name"
        },
        {
            "set": "[User",
            "subset": "Basic",
            "name": "UserName",
            "title": "",
            "content": "[User:Username]",
            "help": "User’s Login User Name"
        },
        {
            "set": "[User",
            "subset": "Profile",
            "name": "Profile",
            "title": "",
            "content": "[Profile:${1:City}]",
            "help": "Use any default or custom Profile Property as listed in Profile Property Definition section of Manage User Accounts. Use non-localized Property Name only."
        },
        {
            "set": "[Environment",
            "subset": "QueryString",
            "name": "QueryString",
            "title": "",
            "content": "[QueryString:${1:ParameterName}]",
            "help": "Value of Querystring Name"
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Now",
            "title": "",
            "content": "[DateTime:Now]",
            "help": "Current Date and Time"
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Ticks-Now",
            "title": "",
            "content": "[Ticks:Now]",
            "help": "CPU Tick Count for Current Second"
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Ticks-Today",
            "title": "",
            "content": "[Ticks:Today]",
            "help": "CPU Tick Count since Midnight"
        },
        {
            "set": "[Environment",
            "subset": "Time",
            "name": "Ticks-Per-Day",
            "title": "",
            "content": "[Ticks:TicksPerDay]",
            "help": "CPU Ticks per Day (for calculations)"
        },
        {
            "set": "[Html",
            "subset": "Resources",
            "name": "script",
            "title": "",
            "content": "<script src=\"[App:Path]/dist/${1:myscripts}.js\" type=\"text/javascript\" data-enableoptimizations=\"100\"></script>",
            "help": ""
        },
        {
            "set": "[Html",
            "subset": "Resources",
            "name": "css, style-sheet",
            "title": "",
            "content": "<link rel=\"stylesheet\" href=\"[App:Path]/assets/AppCatalog.css\" data-enableoptimizations=\"100\"/>",
            "help": ""
        },
        {
            "set": "@Html",
            "subset": "Resources",
            "name": "script",
            "title": "",
            "content": "<script src=\"@App.Path/dist/${1:myscripts}.js\" type=\"text/javascript\" data-enableoptimizations=\"100\"></script>",
            "help": ""
        },
        {
            "set": "@Html",
            "subset": "Resources",
            "name": "css, style-sheet",
            "title": "",
            "content": "<link rel=\"stylesheet\" href=\"@App.Path/assets/AppCatalog.css\" data-enableoptimizations=\"100\"/>",
            "help": ""
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-library",
            "name": "simple loop for assets",
            "title": "",
            "content": "@foreach(var ${3:pic} in AsAdam(${1:var}, \"${2:prop}\").Files){\r\n <span>@${3:pic}.Url, @${3:pic}.FileName </span>\r\n}",
            "help": "Adam: simple example with looping ADAM assets"
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-library",
            "name": "loop with metadata assets",
            "title": "",
            "content": "@foreach(var ${3:pic} in AsAdam(${1:var}, \"${2:prop}\").Files){\r\n <div style=\"clear: both\">\r\n  <img src=\"@${3:pic}.Url?w=200&h=200&mode=crop\" title=\"@${3:pic}.FileName\" style=\"float: right\">\r\n  <h3>@${3:pic}.Metadata.${10:Title}</h3>\r\n  Has Meta: @${3:pic}.HasMetadata \r\n  <div>Description: @Html.Raw(${3:pic}.Metadata.${11:Description})</div>\r\n </div>\r\n}",
            "help": "Adam: Large example with looping ADAM assets"
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-library",
            "name": "loop with type filter",
            "title": "",
            "content": "@foreach(var ${3:pic} in (AsAdam(${1:var}, \"${2:prop}\").Files as IEnumerable<ToSic.SexyContent.Adam.AdamFile>).Where(f => f.Type == \"${4:image}\")){\r\n <span>@${3:pic}.Url, @${3:pic}.FileName </span>\r\n}",
            "help": ""
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail url",
            "title": "",
            "content": "@${101:var}.${102:prop}?w=${1:200}&h=${2:200}&mode=${3:crop}",
            "help": "Thumbnail URL with crop-mode"
        },
        {
            "set": "@\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail IMG tag",
            "title": "",
            "content": "<img src=\"@${101:var}.${102:prop}?w=${1:200}&h=${2:200}&mode=${3:crop}\">",
            "help": "Thumbnail IMG tag with crop-mode"
        },
        {
            "set": "@\\InputType",
            "subset": "string-wysiwyg",
            "name": "raw html",
            "title": "",
            "content": "@Html.Raw(${101:var}.${102:prop})",
            "help": "Output the html as html, not as text"
        },
        {
            "set": "[\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail url",
            "title": "",
            "content": "[${101:var}:${102:prop}]?w=${1:200}&h=${2:200}&mode=${3:crop}",
            "help": "Thumbnail URL with crop-mode"
        },
        {
            "set": "[\\InputType",
            "subset": "hyperlink-default",
            "name": "thumbnail IMG tag",
            "title": "",
            "content": "<img src=\"[${101:var}:${102:prop}]?w=${1:200}&h=${2:200}&mode=${3:crop}\">",
            "help": "Thumbnail IMG tag with crop-mode"
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "content block with inpage editing",
            "title": "",
            "content": "<div class=\"sc-content-block-list\" @${101:var}.${102:prop}.EditContext()>\r\n    @foreach(var contentBlock in ${101:var}.${102:prop}){\r\n        @contentBlock.Render()\r\n    }\r\n</div>\r\n",
            "help": "Content blocks with in-page editing"
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "render entity",
            "title": "",
            "content": "@${101:var}.render()",
            "help": "Render a content-block entity (remember that you can't render the list, this only renders one item).\r\nNote that if you try to render any kind of entity, this will not throw an error, but just render an HTML comment as there is no definition for how to render other types of entities as of now."
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "loop through items",
            "title": "",
            "content": "@foreach(var ${103:item} in ${101:var}.${102:prop}){\r\n    @${103:item}.Render()\r\n}\r\n",
            "help": ""
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "edit context",
            "title": "",
            "content": "@${101:var}.${102:prop}.EditContext()",
            "help": "This should be used inside a <div> tag to provide additional information to the in-page editing ui"
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "item count",
            "title": "",
            "content": "@${101:var}.${102:prop}.Count",
            "help": "This will give you the count of items"
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "get item number 0",
            "title": "",
            "content": "@${101:var}.${102:prop}[0]",
            "help": "Note that this only works, if there are items, otherwise you'll get an error"
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "get first item",
            "title": "",
            "content": "@${101:var}.${102:prop}[0]",
            "help": "This also requires a @using Tosic.SexyContent; at the beginning of your cshtml"
        },
        {
            "set": "@\\InputType",
            "subset": "entity",
            "name": "show title of first item if exists",
            "title": "",
            "content": "@(${1:var}.${2:prop}.Count > 0 ? ${1:var}.${2:prop}[0].Title : \"\")",
            "help": "This is a shorthand for try-to-get-and-if-null-don’t-show"
        },
        {
            "set": "@\\InputType",
            "subset": "entity-content-blocks",
            "name": "render first if it exists",
            "title": "",
            "content": "@(${1:var}.${2:prop}.Count > 0 ? ${1:var}.${2:prop}[0].Render() : \"\")",
            "help": "This is a shorthand for try-to-render-first-and-if-null-don’t-show"
        }
    ]
}