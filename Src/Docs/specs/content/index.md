---
uid: Specs.Content.Index
---

# Content Data Models

Data is just numbers - sometimes assembled together to form text, images or whatever. But to give meaning it must be structured in a way to become **Content**. 

> [!NOTE]
> The smallest piece of _Content_ is an **Entity** - similar to the idea of a record, object or item. 
> You can read more about [Entities here](xref:Specs.Data.Entities).  
>
> The term _Entity_ comes from the underlying data model EAV (Entity/Attribute/Value). 

> [!TIP]
> This information in this document may feel a bit technical. 
> If you're just getting started, you probably don't need this. 



## Content Blocks

Content items wouldn't do much - they must be shown to the user in the intended layout. The configuration of such a _show these things using this template_ is handled in a **ContentBlock**. The data model for these _Content Blocks_ looks like this:

<img src="/images/content/content-block.png" width="100%">

So each _Content Block_ has:

1. One reference to a View configuration
1. Zero, one or many references to content items
1. The same amount of Content-Presentation items 
1. Zero or one references to Header items
1. The same amount of Header-Presentation items


> [!TIP]
> Content Blocks contain a reference to the _View_ and optionally a bunch of _Content Items_ that will be used/shown in that _View_. But there are actually 4 possible scenarios deciding what is actually shown:
>
> 1. The _View_ can show the content-items provided by the _Content Block_
> 1. The _View_ can be configured to use a _Query_ and show data from that
> 1. The _Template_ code could also get data from the _App_ directly and show that
> 1. Combinations of the three options above are possible

> [!IMPORTANT]
> Since the _View_ can also be configured to get data from other sources, it may show items that are not in the list of the _Content Block_. 



## Content Blocks in a CMS like DNN

When you see 2sxc data in DNN, that's because a module was added to the page pointing to a _Content Block_. If you want to know more about that, read [](xref:Specs.Content.DnnIntegration).



## Inner Content

_Content Blocks_ are usually added to pages as [DNN Modules](xref:Specs.Content.DnnIntegration). But there is another way: as **Inner Content**. What this means is that a **Content Item** like a Blog-Post says "Show this other content-block right...here". To help 2sxc keep track of what is used where, this relationship is stored as a **Content Block Reference**. 

<img src="/images/content/content-block-reference.png" width="100%">



## Inner Content from Another App

Usually an _App_ is self-contained, so everything it shows comes from the same _App_. 
This also means that exporting/importing an app will result in the same stuff arriving at the destination. 

The following diagram shows how Content can come from multiple apps

<img src="/images/content/content-block-reference-other-app.png" width="100%">

> [!IMPORTANT]
> So if you're using _Inner Content_ and referencing content from another _App_, this 
> crosses the borders between apps, and you'll need to export/import both _Apps_ to get the same result. 
