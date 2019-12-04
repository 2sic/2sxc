---
uid: Specs.Data.Inputs.Hyperlink
---
# Field Input-Type **hyperlink** / **files**

The **hyperlink (links/files)** field is the base type for input-fields storing [hyperlink](xref:Specs.Data.Values.Hyperlink). It has sub-types.

## Features 
The basic **hyperlink (links/files)** field is used for normal links, page or file/image references as well as for complete sets of files (like image galleries)

## Sub-Types of Hyperlink Fields

1. [hyperlink-default](xref:Specs.Data.Inputs.Hyperlink-Default) - it's input field for normal links, page or file/image references. In menu it has ADAM (automatic digital asset manager), page picker, image manager and file manager.
1. [hyperlink-library](xref:Specs.Data.Inputs.Hyperlink-Library) - it's for complete sets of files (like image galleries).

## Shared Settings
All Hyperlink-Field Types have the following settings:

<img src="/assets/fields/hyperlink/hyperlink.png" width="100%">

* **File Filter** - list of extensions allowed in file/image picker. Example: *.jpg,*.bmp,*.png,*.gif
* **Paths** - this is only needed if you use the old file pickers (not ADAM). Root paths for the picker dialog - ideal if you want all images for this Content-Type to be in the same folder
  
  1. use the syntax foldername - without "/" in front to specify a subfolder of the portal-root. Examples are "Apps" or "Gallery" or "Employees/Photos"
  2. you can also use subfolders - that would be "Employees/Photos"
  3. always remember that this folder must already exist, and DNN must know that it is visible (readable) by the editing user. There are cases where the security settings were not set correctly in dnn - then the file picker won't show anything.

* **Default Dialog** - none, ADAM, page picker, image manager or file manager
* **Show Page Picker** - show page picker in the drop-down 
* **Show Image Manager** - show image manager in the drop-down 
* **Show File Manager** - show file manager in the drop-down 
* **Show Adam** - show the ADAM (automatic digital asset manager) in the drop-down menu.
* **Buttons** - will let you specify which buttons are visible directly. The default is "adam,more" but you could also do "adam,page,more" or just "page". File / image are currently not supported, because as of now, DNN doesn't have a good image/file browser so we discourage its use. 


## History

1. Introduced in EAV 1.0 / 2sxc 1.0
2. Changed in EAV 3.0 / 2sxc 6.0 (it used to have many configuration fields for all kinds of uses, which were then moved to sub-types)
