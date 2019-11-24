---
uid: HowTo.Razor.CustomizeSearch
---
# Event _CustomizeSearch()_ on the Razor Page
## Purpose / Description
This event is called by the view-engine _after_ calling [CustomizeData](xref:HowTo.Razor.CustomizeData) and before passing the `Data` object to the DNN Search Indexer. 

You can override this event to change how data is presented to the search, for example by bundling items together, or by giving items different URLs so that search knows that they are to appear on a sub-page. 

## How to use
In your razor page (.cshtml file) you can add a script block implementing this, as follows:

```c#
@functions
{
    // this method is optional - your code wouldn't need it, but it's in here to show how it would work together
    // the CustomizeData would be called first, and potentially modify what is in the Data-object
    public override void CustomizeData()
    {
        // Don't customize anything, nothing to customize in this case
    }

    /// <summary>
    /// Populate the search - ensure that each entity has an own url/page
    /// </summary>
    /// <param name="searchInfos"></param>
    /// <param name="moduleInfo"></param>
    /// <param name="startDate"></param>
    public override void CustomizeSearch(Dictionary<string, List<ToSic.SexyContent.Search.ISearchInfo>> searchInfos, DotNetNuke.Entities.Modules.ModuleInfo moduleInfo, DateTime startDate)
    {
        foreach (var si in searchInfos["Default"])
        {
            si.QueryString = "mid="+ moduleInfo.ModuleID + "&feature=" + si.Entity.EntityId;
        }
    }
}

```
The code above will skip customizing any data (but often you would want that too), then CustomizeSearch modifies the list of search-items before they are indexed. 

## How it works
In general everything will work automatically. This is what happens:

1. 2sxc will retrieve the data added to this module
2. 2sxc will call the [CustomizeData()](xref:HowTo.Razor.CustomizeData) event if the template has such an event. In this event, your code can add more data to the module as needed.
    1. Note that during the search index, no Request-variables exist.
    1. So your method will cause an error if it does something like var x = Request["Category"].
    1. In case of an error, the index will still continue to work, but your changes to the data will fail
    1. To help you with this, a new property called InstancePurpose was added. It tells you if this view/template was created for displaying or for indexing.
1. 2sxc will then use the data and create SearchItems, ready to index.
    1. Each entity will be turned into a SearchItem
    1. Each Content-Type will have an own list (so you can differentiate between all the SearchItems for the Categories and the SearchItems for the Questions)
    1. Multi-Language is handled correctly, so the English index will contain the English content, etc.
1. 2sxc will then call a CustomizeSearch() event, so your code could provide changes.
    1. A common scenario is to say that each entity (say each question) has a different URL (say a details-page).
    1. So even though all entities belong to the module (and DNN only knows of this one module), the module can say that each entity has an own details page.
1. One this is done, the SearchItems are converted to official SearchDocument-objects and handed over to DNN


## Read also
* [InstancePurpose](xref:HowTo.Razor.Purpose) - which tells you why the current code is running so you could change the data added
* [CustomizeData](xref:HowTo.Razor.CustomizeData)

## Demo App and further links
You should find some code examples in this demo App
* [FAQ with Categories](http://2sxc.org/en/apps/app/faq-with-categories-and-6-views)

More links: [Description of the feature on 2sxc docs](http://2sxc.org/en/Docs-Manuals/Feature/feature/2683)


## History
1. Introduced in 2sxc 6.2
2. Added support for newer DNN versions at a later time - not sure when
