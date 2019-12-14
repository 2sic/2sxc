
# This is the EAV & 2sxc API Documentation v10.21

This is the documentation for EAV and 2sxc APIs. It's fairly advanced, so if you're new to 2sxc, best start with [2sxc.org](https://2sxc.org/).

## 2sxc - The CMS / CMF of DNN (DotNetNuke)
2sxc is a CMS-Plugin for [DNN/DotNetNuke](http://www.dnnsoftware.com/). It makes content editing easier than Wordpress. It's also a [CMF](https://en.wikipedia.org/wiki/List_of_content_management_frameworks) like Drupal. And a very cool EAV data-management system. And an online REST JSON database. And a lot more.

2sxc is basically used to provide web-site-builder functionality to DNN, both in a simple **Content** mode (where users just add images, designed text/image blocks, links etc.) or in **App** mode, where user add standalone functional apps like blogs, galleries and more. Most apps are on github and can be downloaded from the [app-catalog](https://2sxc.org/en/apps).


[!include["Where to Start"](shared/where-to-start.md)]

## Overview and Links to Other Documentations
2sxc is very flexible and can be used for almost any kind of content-management needs or app development needs. Because of this, different documentations help you work with different things. Here's the overview:

1. [2sxc.org](https://2sxc.org)
1. [Getting started tutorials](http://2sxc.org/en/Learn) are found in the _Learn_ section of [2sxc.org](http://2sxc.org/en/)  
includes installation, basic instructions, creating your first content-type and similar tutorials &  
it also include instructions how to export/import data, working with the image resizer and more.
1. A [list of features](http://2sxc.org/en/docs), concepts and examples can be found in the _Docs_ section of 2sxc.org, including things not specifically code related
1. [Many solutions and recommendations](http://2sxc.org/en/blog) are found in the _Blog_ on 2sxc.org
1. [Many demo, template and tutorial Apps](http://2sxc.org/en/Apps) are found in the _App Catalog_ on 2sxc.org
1. [2sxc / EAV Roadmap](xref:Specs.Roadmap.Roadmap)
1. APIs and similar are found in this [wiki](https://github.com/2sic/2sxc/wiki) - you'll see the TOC in the box to the right
1. [App folder structure](xref:Specs.App.Folders) and special files
1. [Style Guide 2020 (beta)](xref:Specs.StyleGuide2020) best-practices draft


## If you want to know more, then check out...

1. The [Specs](xref:Specs.Home) which tells you about Architecture, Data Models and more
1. The [C# .net API Docs](/api/dot-net/index.html)


#### Documentation Version

Generated for EAV/2sxc 10.23.00



#### Pending Work / Tasks

Next to document

1. Finish metadata documentation

These topics are not or insufficiently documented...

1. URL and REST API for retrieving / changing data (todo)
1. Enhancing 2sxc with custom extensions
    1. Custom input field-types
    2. Custom data-types
    5. ...
1. Future topics, lower priority
    1. Angular2-5 and 2sxc (todo)
    2. React and 2sxc (todo)
    4. jQuery with 2sxc (todo)
1. etc.

These topics are already documented elsewhere, but should become part of these docs

1. [View-switching based on url-params](http://2sxc.org/en/Docs/Feature/feature/4680)
1. [Security protecting views like admin-views](http://2sxc.org/en/Docs/Feature/feature/4737)
1. [creating custom DataSource objects](https://2sxc.org/en/blog/tag/datasource)
