---
uid: Specs.App.Folders
---

# Folder structure in the _Content_ App folder

The content-app is simpler than all other apps, because it provides less features to stay focused on "normal" content. 

As such, there are no predefined folders. You can create your own to organize your templates as you need, but the structure is completely undefined. 

## Folder structure and special files of a 2sxc App
A 2sxc app can have no folders at all, or hundreds. The following folders and files are special though, so you should know about them when you need them. 

Note that all apps are located in _\[portal-root\]\2sxc\\\[app-name\]_.

## System Folders

1. **api** this folder contains c# files for the web services this app has
2. **node_modules** is the default folders when you use JS-automation while developing; it can be very large. This folder will be ignored when you export an app
3. **bower_components** contains bower (run-time) dependencies for your JS and can become very large. Normally you will not want this in your app (because it contains a lot of unneeded stuff) so it too will not be exported when you create an app-package. 

## Recommended sub folder names

The following folders have no technical relevance, but we recommend this naming to improve consistency.
1. **src** and sub-folders should contain your javascript source files in original (unminified, etc.)
1. **dist** should contain your processed, minified, uglified and combined JS files

## Special files

1. **app-icon.png** this file is an icon for the app - and will soon be displayed in various dialogs. It should be square and at least 250x250, larger is better

See also [](xref:Specs.App.Icons)