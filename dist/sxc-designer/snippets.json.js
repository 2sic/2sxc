{
    "snippets": [
        {
            "set": "App",
            "key": "Path",
            "title": "returns the url to the current app",
            "tabTrigger": "path",
            "content": "@App.Path",
            "help": "path for integrating scripts,  images etc. For example  use as @App.Path/scripts/knockout.js"
        },
        {
            "set": "App",
            "key": "PhysPath",
            "title": "physical path",
            "tabTrigger": "physical path",
            "content": "@App.PhysicalPath",
            "help": "physical path in c:\\"
        },
        {
            "set": "App",
            "key": "AppGuid",
            "title": "App Guid",
            "tabTrigger": "app guid",
            "content": "@App.AppGuid",
            "help": "internal GUID - should stay the same across all systems for this specific App"
        },
        {
            "set": "App",
            "key": "AppId",
            "title": "App Id",
            "tabTrigger": "app id",
            "content": "@App.AppId",
            "help": "Id in the current data base. Is a different number in every App-Installation"
        },
        {
            "set": "App",
            "key": "AppName",
            "title": "App Name",
            "tabTrigger": "app name",
            "content": "@App.Name",
            "help": "internal name"
        },
        {
            "set": "App",
            "key": "AppFolder",
            "title": "folder of the 2sxc-app",
            "tabTrigger": "app folder",
            "content": "@App.Folder",
            "help": "often used to create paths to scripts or join some values. if you only need to reference a script,  please use App.Path"
        }
    ]
}