---
uid: Specs.Adam.Intro
---

# ADAM - Automatic Digital Asset Management

In a CMS environment, images and PDFs usually belong to a content-item. The user shouldn't have to worry about saving it in the right place. 
Ideally it's only used there, and when the content becomes obsolete, so do the assets that belong to it. 

> [!TIP]
> This is what ADAM is all about. It allows editors to just add images and files. ADAM will take these and magically store it somewhere so the user doesn't have to worry about folders and naming. 

> [!WARNING]
> Since ADAM does everything behind the scenes, it's recommended that users don't try to link to files of another content-item, because that would defeat the purpose. 
> It would also not allow any clean-up in future, because you wouldn't know if the asset was re-used. 

## How ADAM works

Internally ADAM creates a folder for each Entity (Content-Item) using the GUID of that item. 
It also creates a sub-folder for each field, so that a `logo.png` used in the Logo-field is separate from a `my-case-study.pdf` used in the CaseStudy-field. 

The final folder structure looks like this

`[portal-root]/adam/[app-folder-name]/[compact-entity-guid]/field/`

## How to Use

ADAM is a standard part of 2sxc, it's automatically in use everywhere. Any link/file or wysiwyg field support drag-and-drop and will automatically store the file in the right place. 

## Security Concerns

ADAM uses the DNN file-numbering system by default, and keeps a reference to `file:27` in the field linking such a file. In rare situations you may have many untrusted editors, where you want to prevent them asking the API for `file:1`, `file:2`, `file:3` etc. There is an advanced switch to only allow resolving the file number if it's really part of the current item. Contact 2sxc for support on this. 

## Also Read

* [](xref:ToSic.Sxc.Adam)
* To access ADAM files/links in a RazorTemplate, check out `AsAdam(...)` in the [](xref:ToSic.Sxc.Dnn.RazorComponent) and [](xref:ToSic.Sxc.Dnn.ApiController)


## History

1. General Tokens introduced in 2sxc 8.0
1. Added extra security switch in 2sxc 9.32
