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

Understanding how this ties in helps you make better decisions. 



## Also Read about Content Data Model

Best also read about [](xref:Specs.Content.Index) in general. 