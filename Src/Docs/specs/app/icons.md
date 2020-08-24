---
uid: Specs.App.Icons
---

# Icons in Apps

App icons are convention based, so there is no configuration for it. 
To give your app an icon, place a file called `app-icon.png` in the root folder of your app. 
It should be square, and at least 200x200 Pixels, but we recommend it's at least 500x500 pixels. 

## Icons for Views / Templates

This is also convention based, there is no configuration for it. 
To give your views/templates an icon, add an icon file with the same name as your template file. 
So if your template is called `_overview.cshtml` your icon should be `_overview.png`.

## Icons for Content-Types

Content-Types don't have a folder dedicated to them, so this is configuration based. Just edit the Content-Type Metadata and on the Icon-field, drop the image you want for your content-type. 