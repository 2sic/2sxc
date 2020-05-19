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

Best also read about [](xref:Specs.Content.Index) in general. 