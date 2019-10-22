{
  "Content": {
    "Title": "Content and Content-Presentation",
    "Sets": [
      {
        "Id": "content",
        "Title": "Content"
      },
      {
        "Title": "Additional Content Codes",
        "Items": [
          { "[Content:Toolbar]": "Toolbar for inline editing with 2sxc. If used inside a &lt;div class&equals;&quot;sc-element&quot;&gt; then the toolbar will float" }
        ]
      },
      {
        "Id": "presentation",
        "Title": "Presentation"
      }
    ]
  },
  "List": {
    "Title": "List and List-Presentation",
    "Sets": [
      {
        "Id": "list",
        "Title": "List Header",
        "Items": [
          { "[ListContent:Toolbar]": "Outputs the toolbar to edit list information" }
        ]
      },
      {
        "Title": "Important List Codes",
        "Items": [
          { "<repeat repeat=\"Employee in Data:Default\">...[Employee:...]...</repeat>": "Allows defining the repeating part of the template." },
          { "[Content:Repeater:Index]": "Index of the current item" },
          { "[Content:Repeater:Index1]": "Index of the current item + 1 (for numbering lists)" },
          { "[Content:Repeater:Count]": "Count of items in the list" },
          { "[Content:Repeater:IsFirst]": "Outputs <b>First<\/b> if current item is the first one" },
          { "[Content:Repeater:IsLast]": "Outputs <b>Last<\/b> if current item is the last one" },
          { "[Content:Repeater:Alternator2]": "Outputs <b>0<\/b> or <b>1<\/b> depending on items index" },
          { "[Content:Repeater:Alternator3]": "Outputs <b>0<\/b>, <b>1<\/b> or <b>2<\/b> depending on items index" },
          { "[Content:Repeater:Alternator4]": "Outputs <b>0<\/b>, <b>1<\/b>, <b>2<\/b> or <b>3<\/b> depending on items index" },
          { "[Content:Repeater:Alternator5]": "Outputs <b>0<\/b>, <b>1<\/b>, <b>2<\/b>, <b>3<\/b> or <b>4<\/b> depending on items index" },
          { "more...": "<a href&#61;\"http:\/\/2sxc.org\/help\">Learn more<\/a>=More help on 2sxc.org\/help" }
        ]
      },
      {
        "Id": "listpresentation",
        "Title": "List Presentation"
      }
    ]

  },

  "AdditionalRazorSets": { "Text": "SystemRazor,ListRazor,AppRazor" },
  "AdditionalTokenSets": { "Text": "SystemTokens,ListTokens,AppTokens,PortalTokens,TabTokens,ModuleTokens,QueryStringTokens,UserTokens,ProfileTokens" },
  "AppRazor": { "List": "@App.Path=returns the url to the current app, for integrating scripts, images etc. For example, use as @App.Path\/scripts\/knockout.js\n@App.PhysicalPath=physical path, in c:\\\n@App.AppGuid=internal GUID - should stay the same across all systems for this specific App\n@App.AppId=ID in the current DB. Is a different number in every App-Installation\n@App.Name=internal name\n@App.Folder=folder\n@App.Resources.ANYKEY=any resource defined in the App - usually used for multilanguage labels like @App.Resources.FirstName\n@App.Settings.ANYKEY=any type of setting defined in the App - usually used for configuration like details-pages @App.Settings.DetailsLink" },
  "AppTokens": { "List": "[App:Path]=returns the url to the current app, for integrating scripts, images etc. For example, use as [App:Path]\/scripts\/knockout.js\n[App:PhysicalPath]=the physical path, in case you want access to the C:\\path for some reason\n[App:Resources:...]=any resource defined in the App - usually used for multilanguage labels like [App:Resources:FirstName]\n[App:Settings:...]=any type of setting defined in the App - usually used for configuration like details-pages [App:Settings:DetailsLink]" },
  "ListRazor": { "List": "@ListContent.Toolbar«Toolbar for inline editing with 2sxc. If used inside a <div class=\"sc-element\"> then the toolbar will float»\n\t<ul>\n\t@foreach(var e in List) {\n\t\tvar Content = e.Content;\n\t\t<li class=\"sc-element\">\n\t\t\t@Html.Raw(Content.Toolbar)\n\t\t\t@Content.EntityTitle\n\t\t<\/li>\n\t}\n\t<\/ul>«Demo-Loop how to cycle through items»" },

  "ModuleTokens": {
    "List": "[Module:Description]=Module Definition Description \n[Module:EndDate]=Module Display Until Date \n[Module:Footer]=Module Footer Text \n[Module:FriendlyName]=Module Definition Name \n[Module:Header]=Module Header Text \n[Module:HelpURL]=Module Help URL \n[Module:IconFile]=Module Path to Icon File \n[Module:ModuleTitle]=Module Title \n[Module:PaneName]=Module Name of Pane (where the module resides) \n[Module:StartDate]=Module Display from Date",
    "Title": "Module Tokens"
  },
  "PortalTokens": {
    "List": "[Portal:Currency]=Currency String \n[Portal:Description]=Portal Description \n[Portal:Email]=Portal Admin Email \n[Portal:FooterText]=Portal Copyright Text \n[Portal:HomeDirectory]=Portal Path (relative) of Home Directory \n[Portal:LogoFile]=Portal Path to Logo File \n[Portal:PortalName]=Portal Name \n[Portal:PortalAlias]=Portal URL \n[Portal:TimeZoneOffset]=Difference in Minutes between Portal Default Time and UTC",
    "Title": "Portal Tokens"
  },
  "ProfileTokens": {
    "List": "[Profile:]=Use any default or custom Profile Property as listed in Profile Property Definition section of Manage User Accounts. Use non-localized Property Name only.",
    "Title": "User Profile Tokens"
  },
  "QueryStringTokens": {
    "List": "[Querystring:Name]=Value of Querystring Name",
    "Title": "QueryString (URL-Parameters) Tokens"
  },
  "SystemRazor": { "List": "@Content.Toolbar=SexyContent Toolbar" },

  "TabTokens": {
    "List": "[Tab:Description]=Page Description Text for Search Engine \n[Tab:EndDate]=Page Display Until Date \n[Tab:FullUrl]=Page Full URL \n[Tab:IconFile]=Page Relative Path to Icon File \n[Tab:KeyWords]=Page Keywords for Search Engine \n[Tab:PageHeadText]=Page Header Text \n[Tab:StartDate]=Page Display from Date \n[Tab:TabName]=Page Name \n[Tab:TabPath]=Page Relative Path \n[Tab:Title]=Page Title (Window Title) \n[Tab:URL]=Page URL",
    "Title": "Tab (Page) Tokens"
  },
  "TimeTokens": {
    "List": "[DateTime:Now]=Current Date and Time \n[Ticks:Now]=CPU Tick Count for Current Second \n[Ticks:Today]=CPU Tick Count since Midnight \n[Ticks:TicksPerDay]=CPU Ticks per Day (for calculations)",
    "Title": "Time Tokens"
  },
  "UserTokens": {
    "List": "[User:DisplayName]=User’s Display Name \n[User:Email]=User’s Email Address \n[User:FirstName]=User’s First Name \n[User:FullName]=(deprecated)\n[User:LastName]=User’s Last Name \n[User:Username]=User’s Login User Name",
    "Title": "User Tokens"
  }
}