---
uid: HowTo.DynamicCode.Dnn
---
# Link / @Dnn Object in Razor / .net

The `Dnn` is a helper object which contains properties to access information about the current tab, portal, user, module etc.


## How to use

Here's a quick example of using the `Dnn` object in a Razor template: 

```html
<!-- show the current users nice name -->
@Dnn.User.DisplayName
```

## How it works
There's not much to explain here, we're just providing the standard DNN objects to the template.


## The Main Properties of the @Dnn Object
These are the main properties:

1. Module  
2. Portal
3. Tab
4. User

## The @Dnn.Module Properties
The `@Dnn.Module` is of the type [ModuleInfo](https://www.dnndocs.com/api/DotNetNuke.Entities.Modules.ModuleInfo.html). It has the following properties:

1.	Dnn.Module.AllModules
2.	Dnn.Module.AllTabs
3.	Dnn.Module.Border
4.	Dnn.Module.Cacheability
5.	Dnn.Module.CacheMethod
6.	Dnn.Module.CacheTime
7.	Dnn.Module.ContainerPath
8.	Dnn.Module.ContainerSrc
9.	Dnn.Module.DefaultCacheTime
10.	Dnn.Module.DesktopModule
11.	Dnn.Module.DesktopModuleId
12.	Dnn.Module.DisplayTitle
13.	Dnn.Module.EndDate - for show/hide
14.	Dnn.Module.Footer
15.	Dnn.Module.Header
16.	Dnn.Module.ModuleControl
17.	Dnn.Module.ModuleControlId
18.	Dnn.Module.ModuleDefID -  note: large D
19.	Dnn.Module.ModuleDefinition
20.	Dnn.Module.ModuleID - probably the most used value, note the large ID
21.	Dnn.Module.ModuleName
22.	Dnn.Module.ModuleOrder
23.	Dnn.Module.ModulePermissions
24.	Dnn.Module.ModuleSettings
25.	Dnn.Module.ModuleTitle
26.	Dnn.Module.PaneModuleCount
27.	Dnn.Module.PaneModuleIndex
28.	Dnn.Module.PaneName
29.	Dnn.Module.ParentTab - a TabInfo, same as Dnn.Tab
30.	Dnn.Module.PortalID - note large D
31.	Dnn.Module.StartDate - for show/hide rules
32.	Dnn.Module.TabID - note large D
33.	Dnn.Module.TabModuleID - note large D
34.	Dnn.Module.TabModuleSettings
35.	Dnn.Module.UniqueId
36.	Dnn.Module.Visibility



## The @Dnn.Portal Properties
The `@Dnn.Portal` is of the type [PortalSettings](https://www.dnndocs.com/api/DotNetNuke.Entities.Portals.PortalInfo.html). It has the following properties:

1.	Dnn.Portal.ActiveTab
2.	Dnn.Portal.AdministratorId
3.	Dnn.Portal.AdministratorRoleId
4.	Dnn.Portal.AdministratoRoleName
5.	Dnn.Portal.AdminTabId
6.	Dnn.Portal.CultureCode
7.	Dnn.Portal.Currency
8.	Dnn.Portal.Current - static method returning the current portal portal-settings
9.	Dnn.Portal.Default... (AdminContainer, AdminSkin, ControlPanelMode, etc.)
10.	Dnn.Portal.DefaultLanguage
11.	Dnn.Portal.DefaultModuleId
12.	Dnn.Portal.DefaultPortalContainer
13.	Dnn.Portal.DefaultTabId
14.	Dnn.Portal.Description
15.	Dnn.Portal.Email
16.	Dnn.Portal.EnableBrowserLanguage
17.	Dnn.Portal.EnableUrlLanguage
18.	Dnn.Portal.ExpiryDate
19.	Dnn.Portal.FooterText
20.	Dnn.Portal.GetProperty(string, string, cultureInfo, UserInfo, scope, boolean) - ?
21.	Dnn.Portal.GUID - notice all large letters
22.	Dnn.Portal.HomeDirectory
23.	Dnn.Portal.HomeDirectoryMapPath
24.	Dnn.Portal.HomeTabId - the root page
25.	Dnn.Portal.KeyWords
26.	Dnn.Portal.LoginTabId - where the normal login usually goes to
27.	Dnn.Portal.LogoFile
28.	Dnn.Portal.Pages
29.	Dnn.Portal.PortalAlias
30.	Dnn.Portal.PortalId - very important - notice the small d
31.	Dnn.Portal.PortalName
32.	Dnn.Portal.PortalSkin
33.	Dnn.Portal.RegisteredRoleId
34.	Dnn.Portal.RegisteredRoleName
35.	Dnn.Portal.RegisterTabId
36.	Dnn.Portal.Search... (various values like IncludeCommon, IncludeTagInfoFilter etc.)
37.	Dnn.Portal.SearchTabId
38.	Dnn.Portal.SiteLogHistory
39.	Dnn.Portal.SplashTabId
40.	Dnn.Portal.SSLEnabled
41.	Dnn.Portal.SSLEnforced
42.	Dnn.Portal.SSLURL - all caps
43.	Dnn.Portal.STDURL - all caps
44.	Dnn.Portal.SuperTabId
45.	Dnn.Portal.TimeZoneOffset
46.	Dnn.Portal.UserId
47.	Dnn.Portal.UserInfo
48.	Dnn.Portal.UserMode
49.	Dnn.Portal.UserQuota
50.	Dnn.Portal.UserRegistration
51.	Dnn.Portal.Users
52.	Dnn.Portal.UserTabId


## The @Dnn.Tab Properties
The `@Dnn.Tab` is of the type [TabInfo](https://www.dnndocs.com/api/DotNetNuke.Entities.Tabs.TabInfo.html). It has the following properties:

1.	Dnn.Tab.ChildModules
2.	Dnn.Tab.ContainerPath
3.	Dnn.Tab.ContainerSrc
4.	Dnn.Tab.CultureCode
5.	Dnn.Tab.DefaultLanguageGuid
6.	Dnn.Tab.DefaultLanguageTab
7.	Dnn.Tab.Description
8.	Dnn.Tab.EndDate - for show/hide of this tab
9.	Dnn.Tab.FullUrl
10.	Dnn.Tab.HasChildren
11.	Dnn.Tab.IsDefaultLanguage
12.	Dnn.Tab.IsDeleted
13.	Dnn.Tab.IsNeutralCulture
14.	Dnn.Tab.IsSecure
15.	Dnn.Tab.IsTranslated
16.	Dnn.Tab.IsVisible
17.	Dnn.Tab.KeyWords
18.	Dnn.Tab.Level
19.	Dnn.Tab.LocalizedTabName
20.	Dnn.Tab.LocalizedTabs
21.	Dnn.Tab.Modules
22.	Dnn.Tab.PageHeadtext
23.	Dnn.Tab.Panes
24.	Dnn.Tab.ParentId
25.	Dnn.Tab.PermanentRedirect
26.	Dnn.Tab.PortalID - notice the large D
27.	Dnn.Tab.RefreshInterval
28.	Dnn.Tab.SiteMapPriority
29.	Dnn.Tab.SkinDoctype
30.	Dnn.Tab.SkinPath
31.	Dnn.Tab.SkinSrc
32.	Dnn.Tab.StartDate - for show/hide
33.	Dnn.Tab.TabID - the current Tab-number - notice the large D
34.	Dnn.Tab.TabName
35.	Dnn.Tab.TabOrder
36.	Dnn.Tab.TabPath
37.	Dnn.Tab.TabPermissions
38.	Dnn.Tab.TabSettings
39.	Dnn.Tab.TabType
40.	Dnn.Tab.Title
41.	Dnn.Tab.UniqueId - small "d"
42.	Dnn.Tab.Url


## The @Dnn.User Properties
The `@Dnn.User` is of the type [UserInfo](https://www.dnndocs.com/api/DotNetNuke.Entities.Users.UserInfo.html). It has the following properties:

1.	Dnn.User.DisplayName
2.	Dnn.User.Email
3.	Dnn.User.FirstName
4.	Dnn.User.FullName
5.	Dnn.User.IsDeleted
6.	Dnn.User.IsInRole(string RoleName)
7.	Dnn.User.IsSuperUser
8.	Dnn.User.LastName
9.	Dnn.User.Membership - these are asp.net memberships, you probably don't need them
10.	Dnn.User.PortalId
11.	Dnn.User.Roles - these are DNN roles
12.	Dnn.User.UserID - note the large D
13.	Dnn.User.Username



## History
1. Introduced in 2sxc 1.0
2. 



