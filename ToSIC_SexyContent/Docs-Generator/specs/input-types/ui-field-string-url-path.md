---
uid: Specs.Data.Inputs.String-Url-Path
---
# UI Field Type: string-url-path

## Purpose / Description
Use this field type to manage url-paths which you'll usually use to identify an
item. For example, if you have a blog and each post has a url with the name, this is the field that you need to match the url to the item. It stores a [string/text data](xref:Specs.Data.Type.String). It's an extension of the basic [string field type](xref:Specs.Data.Inputs.String).

## Features 

1. ensure that only url-safe characters are used
1. automatically generate a url based on one or many other fields when editing the first time
1. keeps the generated url stable later on
1. also allows manual editing if needed

## Configuring a String-Url-Path
This shows the configuration dialog:

<img src="/assets/fields/string/string-url-path.png" width="100%">

* **Auto Generate...** here you can build a template how the url should be auto-generated
* **Allow Slashes** this let's you choose if slashes are desired in this url-fragment - in most cases you don't want slashes

## Read more

1. Read this [post & watch the video](https://2sxc.org/en/docs/docs/feature/feature/8305) when we introduced it

## History
1. Introduced in EAV 4.0 2sxc 8.3