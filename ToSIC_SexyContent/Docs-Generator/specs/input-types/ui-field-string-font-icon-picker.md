---
uid: Specs.Data.Inputs.String-Font-Icon-Picker
---
# UI Field Type: string-font-icon-picker

## Purpose / Description
Use this field type to create input-fields which let the user pick an icon. It stores a [string/text data](xref:Specs.Data.Type.String). It's an extension of the basic [string field type](xref:Specs.Data.Inputs.String).

## Features 

1. shows all icons from the icon-library
1. allows searching
1. supports libraries using prefixes (like [font-awesome](http://fontawesome.io/), which use `fa-iconname`)
1. supports libraries using double classes (like [glyphicons](https://getbootstrap.com/docs/3.3/components/), which use `glyphicons glyphicons-plus`)
1. supports custom libraries you can build, for example using [fontello](http://fontello.com/) - see [instructions](https://2sxc.org/en/blog/post/create-custom-icon-fonts-in-99-seconds-for-web-sites-video)
1. lets you auto-load more css-files to load icon-definitions on the fly

## Result
This is what it looks like for the user:

<img src="/assets/fields/string/string-font-icon-picker-preview.png" width="100%">

## Configuring a String-Font-Icon-Picker
This shows the configuration dialog:

<img src="/assets/fields/string/string-font-icon-picker.png" width="100%">

* **CSS Prefix** tells the UI to find all css-classes that start with this, and build icons with them
* **Preview CSS Classes** this tells the GUI to add this while showing a preview for icons in the library (so the preview works for Glyphicons etc.)
* **Files** tells the UI to load CSS files
  * _Important_ they should usually be in your project, because the UI-JavaScripts can't scan CSS files loaded from externally, like from a CDN
  * Use the token [App:Path] to ensure that it's always loaded from the right

## Read more

1. Read this [post & watch the video](https://2sxc.org/en/blog/post/using-font-icon-pickers-and-search-in-dynamic-content-types) which we created when we introduced it

## History
1. Introduced in EAV 4.0 2sxc 8.4