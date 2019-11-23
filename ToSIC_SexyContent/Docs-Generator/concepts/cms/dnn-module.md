---
uid: Concepts.DnnModule
---

# Concept: _How DNN Modules Connect To Content-Blocks_ 

## Purpose / Description
This page explains how a DNN module is mapped to content which is shown inside this module. 

## Basic Setup: Content is assigned to the module
This is the most common setup - and used in the **Content** module as well as in many **App** modules, which rely on the author to manually create content for this specific module. It's not usually used in data-oriented modules like news, blogs, etc. See also []

Here is how it's mapped:

<img src="/assets/concepts/how-modules-relate-to-content-groups.png" width="100%">

## Manually Managing this Data
Note: you shoudn't usually do this - but sometimes you have to. Check out this short explanation:

<img src="/assets/concepts/administrating-content-group-in-an-app.jpg" width="100%">



## Notes and Clarifications
As mentioned, this is the default setup. In the case of data-oriented apps, which query data from somewhere, you'll usually have much more data based on that query. Of course that data isn't listed in this _Content-Group_, as it's dynamic. 

## Read also

* [In-Depth information about this](http://2sxc.org/en/blog/post/understanding-content-binding-to-modules-and-pages-(300))



## History

1. Introduced in 2sxc 6, previously it was handled a bit differently

[some-link-name]:Razor-SexyContentWebPage.CustomizeData