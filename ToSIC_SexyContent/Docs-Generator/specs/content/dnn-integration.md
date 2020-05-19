---
uid: Specs.Content.DnnIntegration
---

# 2sxc Content in DNN

The vision of 2sxc is to be cross-platform, so we hope one day it will also run on NopCommerce and other systems. This is how it's integrated into DNN. 

<img src="/images/content/content-in-dnn.png" width="100%">



## What DNN References to

In DNN each modules has _Module Settings_ to store configuration. 2sxc stores these two pieces of information in the Module Settings:

1. App ID - what app is being shown here
1. _Content Block_ ID - the GUID of the _Content Block_ Entity containing the rest of the configuration

Understanding how this ties in helps you make better decisions. Make sure you also read [](xref:Specs.Content.Index)



## Basic Setup: Content is assigned to the module

This is the most common setup - and used in the **Content** module as well as in many **App** modules, which rely on the author to manually create content for this specific module. It's not usually used in data-oriented modules like news, blogs, etc. See also []

Here is how it's mapped:

<img src="/assets/concepts/how-modules-relate-to-content-groups.png" width="100%">



## Manually Managing this Data

Note: you shoudn't usually do this - but sometimes you have to. Check out this short explanation:

<img src="/assets/concepts/administrating-content-group-in-an-app.jpg" width="100%">

> [!NOTE]
> This scerenshot above is from 2sxc 8. In 2sxc 11 you can change scopes in the dropdown below the data table.


## FAQ

1. If a page or module is deleted, does it also delete the _Content Block_?  
No. Note that if a page or module is deleted, it goes into the DNN trash, so it could always be restored again. 
1. If a page or module is deleted from the trash, does it also delete the _Content Block_?  
No. DNN does not inform modules about delete actions, so we can't do clean-up.
1. Are _Content Blocks_ which don't appear on a page orphaned and can I delete them?  
Maybe. Since they could be used in other apps (see [](xref:Specs.Content.Index)) as _Inner Content_, there is no quick way to tell if it's being used elsewhere.
1. Can a _Content Block_ be used on multiple modules / pages?  
Yes. It's not common, but since a module can be shown on multiple pages or even on other portals, it would show the same _Content Block_ there as well. 


## Also Read about Content Data Model

* Best also read about [](xref:Specs.Content.Index) in general. 
* [Blog about the internals of modules and content](https://2sxc.org/en/blog/post/understanding-content-binding-to-modules-and-pages-(300))


## History

1. Introduced in 2sxc 6, previously it was handled a bit differently
1. 2sxc 11.02 added a feature to see where views are in use